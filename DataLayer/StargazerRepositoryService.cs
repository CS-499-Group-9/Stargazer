using System.Collections.Concurrent;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using DataLayer.EquatorialObjects;
using DataLayer.Implementations;
using System.Diagnostics;
using CosineKitty;

namespace DataLayer
{
    /// <summary>
    /// Used to make requests to convert data from the equatorial coordinate system to the horizontal coordinate system and receive a data package containing all values.
    /// </summary>
    /// <typeparam name="T">The class type of the star to be drawn by the front end. 
    /// This is needed for the internal logic of the <see cref="CelestialDataPackage{T}"/></typeparam>
    public class StargazerRepositoryService<T>
    {
        /// <summary>
        /// Used to filter the maximum magnitude of stars retrieved from the Yale Star repository
        /// </summary>
        // const double maxStarMagnitude = 15;
        private const string Value = "Messier Complete";

        /// <summary>
        /// Immutable in-memory collection of all stars that were retrieved during initialization (excluding stars in constellations)
        /// </summary>
        private readonly IEnumerable<EquatorialStar> equatorialStars;
        /// <summary>
        /// Immutable collection of all constellations retrieved during initialization.
        /// </summary>
        private readonly IEnumerable<Constellation> constellations;

        /// <summary>
        /// Immutable collection of all Messier Deep Space Objects retrieved during initialization.
        /// </summary>
        private readonly IEnumerable<EquatorialMessierObject> equatorialMessierObjects;

        /// <summary>
        /// Producer-consumer collection of <see cref="HorizontalStar"/>s passed to the graphical layer
        /// </summary>
        private IEnumerable<HorizontalStar> horizontalStars;
        /// <summary>
        /// Producer-consumer collection of converted <see cref="HorizontalMessierObject"/>s passed to the graphical layer
        /// </summary>
        private IEnumerable<HorizontalMessierObject> horizontalMessierObjects;

        /// <summary>
        /// Dictionary of <see cref="HorizontalStar"/>s contained in constellations 
        /// </summary>
        private ConcurrentDictionary<int, HorizontalStar> ConstellationStars { get;  }
        private CelestialDataPackage<T> CelestialDataPackage;

        /// <summary>
        /// Used by the <see cref="StargazerRepositoryService{T}.CreateAsync(IStarRepository, IConstellationRepository, IMessierRepository)"/> method to initialize a new service
        /// </summary>
        /// <param name="equatorialStars">The stars retrieved from the repository</param>
        /// <param name="equatorialConstellations">The constellations retrieved from the repository</param>
        /// <param name="equatorialMessierObjects">The Messier Deep Space Objects retrieved from the repository</param>
        private StargazerRepositoryService(
            IEnumerable<EquatorialStar> equatorialStars,
            IEnumerable<Constellation> equatorialConstellations,
            IEnumerable<EquatorialMessierObject> equatorialMessierObjects)
        {
            this.equatorialStars = equatorialStars;
            this.constellations = equatorialConstellations;
            this.equatorialMessierObjects = equatorialMessierObjects;

            // Create new empty concurrent collections
            horizontalStars = new BlockingCollection<HorizontalStar>(new ConcurrentBag<HorizontalStar>());
            horizontalMessierObjects = new BlockingCollection<HorizontalMessierObject>(new ConcurrentBag<HorizontalMessierObject>());
            ConstellationStars = new ConcurrentDictionary<int, HorizontalStar>();
        }

        /// <summary>
        /// Asynchronously creates a new instance of <c>StargazerRepositoryService</c>
        /// </summary>
        /// <param name="starRepository">Used to access a repository of Yale Stars</param>
        /// <param name="constellationRepository">Used to access a repository of constellations identified by the HIP ID</param>
        /// <param name="messierRepository">Used to access a repository of Messier Deep Space Objects</param>
        /// <returns>A new <c>StargazerRepositoryService</c> object.</returns>
        public static async Task<StargazerRepositoryService<T>> CreateAsync(
            IStarRepository starRepository,
            IConstellationRepository constellationRepository,
            IMessierRepository messierRepository)
        {
            // Create tasks to retrieve all objects and have them run on separate threads
            var getStars = starRepository.GetAllStarsAsync();
            var getMessierObjects = messierRepository.GetRawMessierObjectsAsync();
            var getConstellations = constellationRepository.GetAllConstellationsAsync();

            // Pull the constellation stars out of the collection and into the dictionary.
            //var equatorialConstellationStars = await GatherConstellationStars(getStars, getConstellations, starRepository);

            // If the Messier Repository has not finished retrieving data, wait for it.
            await Task.WhenAny(getMessierObjects);

            // Retrieve the results of all async calls
            var equatorialStars = getStars.Result;
            var equatorialConstellations = getConstellations.Result;
            var equatorialMessierObjects = getMessierObjects.Result;

            // Get all stars that are a part of constellations and move them into the Constellation stars list

            // Return the fully constructed object
            return new StargazerRepositoryService<T>(
                equatorialStars,
                equatorialConstellations,
                equatorialMessierObjects
            );
        }


        public async Task<CelestialDataPackage<T>> InitializeDataPackage()
        {
            CosineKittyEquatorialCalculator starConverter = new();

            await Task.Run(() =>
            {
                CalculateStars();
            });

            // Calculate the Messier Objects
            await Task.Run(() =>
            {
                CalculateMessierObjects();

            });
            var planets = starConverter.CreatePlanets();
            var moon = starConverter.CreateMoon();
            var sun = starConverter.CreateSun();

            // Pack up the data and ship it back.
            return new CelestialDataPackage<T>(
                horizontalStars, starConverter,
                horizontalMessierObjects,
                constellations,
                new ConcurrentDictionary<int, T>(),
                planets,
                moon, 
                sun);
        }

        private void CalculateMessierObjects()
        {
            List<HorizontalMessierObject> newMessier = new();
            foreach (var item in equatorialMessierObjects)
            {
                var messier = new HorizontalMessierObject(item);
                messier.MessierId = item.MessierId;
                messier.Size = item.Size;
                messier.ViewingDifficulty = item.ViewingDifficulty;
                messier.Constellation = item.Constellation;
                messier.NewGeneralCatalog = item.NewGeneralCatalog;
                messier.ViewingSeason = item.ViewingSeason;
                newMessier.Add(messier);
            }
            horizontalMessierObjects = newMessier;
        }

        private void CalculateStars()
        {
            List<HorizontalStar> newStars = new();
            foreach (var item in equatorialStars)
            {
                var star = new HorizontalStar(item);
                newStars.Add(star);
            }
            horizontalStars = newStars;
        }
    }
}
          
