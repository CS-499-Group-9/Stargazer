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
    public class StargazerRepositoryService<T> : IRepositoryService<T>
    {

        private readonly IStarRepository starRepository;
        private readonly IMessierRepository messierRepository;
        private readonly IConstellationRepository constellationRepository;


        /// <summary>
        /// Instantiates a new stargazer repository service.
        /// </summary>
        /// <param name="starRepository">The repository service for loading stars.</param>
        /// <param name="constellationRepository">The repository service for loading constellations.</param>
        /// <param name="messierRepository">The repository service for loading Messier Objects</param>
        public StargazerRepositoryService(
            IStarRepository starRepository,
            IConstellationRepository constellationRepository,
            IMessierRepository messierRepository)
        {
            //this.equatorialStars = equatorialStars;
            this.starRepository = starRepository;
            this.constellationRepository = constellationRepository;
            this.messierRepository = messierRepository;
        }

        /// <summary>
        /// Initializes the data package
        /// Should be awaited.
        /// </summary>
        /// <returns>A <see cref="Task{CelestialDataPackage}"/> of type <c>T</c></returns>
        public async Task<CelestialDataPackage<T>> InitializeDataPackage()
        {
            CosineKittyEquatorialCalculator starConverter = new();

            //Start the CPU intensive tasks.
            var starProducer =  Task.Run(() =>
            {
                return CalculateStars();
            });

            var messierProducer =  Task.Run(() =>
            {
                return CalculateMessierObjects();

            });

            // Gather the other objects that do not take a lot of time
            var constellations = constellationRepository.GetConstellations();
            var planets = starConverter.CreatePlanets();
            var moon = starConverter.CreateMoon();
            var sun = starConverter.CreateSun();

            

            // Pack up the data and ship it back.
            // The star and messier producers are awaited in the constructor to get the results. 
            return new CelestialDataPackage<T>(
                await starProducer, 
                starConverter,
                await messierProducer,
                constellations,
                planets,
                moon, 
                sun);
        }

        private IEnumerable<HorizontalMessierObject> CalculateMessierObjects()
        {
            List<HorizontalMessierObject> newMessier = new();
            foreach (var item in messierRepository.GetMessierObjects())
            {
                var messier = new HorizontalMessierObject(item);
                messier.MessierId = item.MessierId;
                messier.Size = item.Size;
                messier.ViewingDifficulty = item.ViewingDifficulty;
                messier.Constellation = item.Constellation;
                messier.NewGeneralCatalog = item.NewGeneralCatalog;
                messier.ViewingSeason = item.ViewingSeason;
                messier.CommonName = item.CommonName;
                newMessier.Add(messier);
            }
            return newMessier;
        }

        private IEnumerable<HorizontalStar> CalculateStars()
        {
            List<HorizontalStar> newStars = new();
            foreach (var item in starRepository.GetAllStars())
            {
                var star = new HorizontalStar(item);
                newStars.Add(star);
            }
            return newStars;
        }
    }
}
          
