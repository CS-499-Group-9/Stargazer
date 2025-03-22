using System.Collections.Concurrent;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using DataLayer.EquatorialObjects;
using DataLayer.Implementations;
using System.Diagnostics;

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
        const double maxStarMagnitude = 6;
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
        /// Immutable dictionary of all stars that are part of known constellations (no repeats, the HipparcosId is used as the key)
        /// </summary>
        private readonly IReadOnlyDictionary<int, EquatorialStar> equatorialConstellationStars;

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

        /// <summary>
        /// Used by the <see cref="StargazerRepositoryService{T}.CreateAsync(IStarRepository, IConstellationRepository, IMessierRepository)"/> method to initialize a new service
        /// </summary>
        /// <param name="equatorialStars">The stars retrieved from the repository</param>
        /// <param name="equatorialConstellations">The constellations retrieved from the repository</param>
        /// <param name="equatorialConstellationStars">The stars retrieved from the repository that are part of constellations</param>
        /// <param name="equatorialMessierObjects">The Messier Deep Space Objects retrieved from the repository</param>
        private StargazerRepositoryService(
            IEnumerable<EquatorialStar> equatorialStars,
            IEnumerable<Constellation> equatorialConstellations,
            IReadOnlyDictionary<int, EquatorialStar> equatorialConstellationStars,
            IEnumerable<EquatorialMessierObject> equatorialMessierObjects)
        {
            this.equatorialStars = equatorialStars;
            this.constellations = equatorialConstellations;
            this.equatorialConstellationStars = equatorialConstellationStars;
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
            var getStars = starRepository.GetAllStarsAsync(maxStarMagnitude);
            var getMessierObjects = messierRepository.GetRawMessierObjectsAsync();
            var getConstellations = constellationRepository.GetAllConstellationsAsync();

            // Pull the constellation stars out of the collection and into the dictionary.
            var equatorialConstellationStars = await GatherConstellationStars(getStars, getConstellations, starRepository);

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
                new Dictionary<int, EquatorialStar>(equatorialConstellationStars),
                equatorialMessierObjects
            );
        }

        /// <summary>
        /// Gathers all stars that are a part of constellations into a separate dictionary using the HipparcosId as the key.
        /// This is because the Hip ID is used by the constellations to list the nodes of the constellation graph.
        /// Removes the stars from the master listing.
        /// </summary>
        /// <param name="starTask">The star repository retrieval task</param>
        /// <param name="constellationTask">The constellation repository retrieval task</param>
        /// <param name="starRepository">The star repository so that constellation stars that were filtered out can be retrieved by HipparcosId</param>
        /// <returns>The dictionary of constellation stars</returns>
        /// <exception cref="InvalidOperationException">If a constellation star cannot be found.</exception>
        private static async Task<IDictionary<int, EquatorialStar>> GatherConstellationStars(
            Task<IList<EquatorialStar>> starTask,
            Task<IList<Constellation>> constellationTask,
            IStarRepository starRepository)
        {
            // Create a new blank dictionary
            IDictionary<int, EquatorialStar> constellationStars = new Dictionary<int, EquatorialStar>();

            // Release the thread until the star and constellation data retrieval has completed
            await Task.WhenAll(starTask, constellationTask);

            // Get the results of the tasks
            IList<EquatorialStar> stars = starTask.Result;
            IList<Constellation> constellations = constellationTask.Result;

            // Start a new thread to perform the work and release the current thread to await the task completion.
            await Task.Run(async () =>
            {
                // Iterate through the constellations
                foreach (var constellation in constellations)
                {
                    // Iterate through the Tuple<int, int> of constellation graph edges
                    foreach (var line in constellation.ConstellationLines)
                    {
                        // Transform the two vertices of the edge into an array
                        var endPoints = new int[] { line.Item1, line.Item2 };

                        // Loop through the vertices
                        foreach (var endPoint in endPoints)
                        {
                            // When an exception is thrown inside the task, it does weird things. I will revisit this. Still not sure I solved it, but there are no more exceptions, so I can't fix what isn't broken
                            try
                            {
                                // See if the star is already in the dictionary. If found, do nothing
                                if (!constellationStars.TryGetValue(endPoint, out EquatorialStar? star))
                                {
                                    // The star is not in the dictionary, get it from the list of stars
                                    star = stars.FirstOrDefault(s => s.HipparcosId == endPoint);
                                    if (star == null)
                                    {
                                        // The star was hopefully filtered out, get it from the repository
                                        star = await starRepository.GetStarByHipAsync(endPoint);
                                        // If the star is still null, throw an exception
                                        if (star == null) throw new InvalidOperationException($"{endPoint} was not found.");
                                    }
                                    // By one method or the other, the star was found. Remove it from the master collection and add it to the dictionary
                                    stars.Remove(star);
                                    constellationStars.TryAdd(endPoint, star);
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                throw new InvalidOperationException($"Test");
                            }
                        }
                    }
                }
            });

            return constellationStars;
        }

        /// <summary>
        /// Makes an async call to convert all data from Equatorial to Horizontal coordinates based on supplied user data.
        /// </summary>
        /// <param name="latitude">The latitude of the observer (-90 &lt;= value &lt;= 90)</param>
        /// <param name="longitude">The longitude of the observer (-90 &lt;= value &lt;= 90)</param>
        /// <param name="localUserTime">The observer time in Universal TimeCode <code cref="DateTime.ToUniversalTime()">new DateTime(year, month, day, hour, min, sec).ToUniversalTime()</code></param>
        /// <returns>A package containing all data required for display. (This call should be made in an <c>async</c> method using the <c>await</c> keyword)</returns>
        public async Task<CelestialDataPackage<T>> UpdateUserPosition(double latitude, double longitude, DateTime localUserTime)
        {
            // Create a Star converter (created outside of the task because it is used for the stars and constellations
            // I really wish all three of these could happen concurrently instead of waiting for one to complete before staring the other, but it causes MAJOR issues
            // Doesn't really make sense to me...

            CosineKittyEquatorialConverter<HorizontalStar> starConverter = new(latitude, longitude, localUserTime);
            CosineKittyEquatorialConverter<HorizontalMessierObject> converter = new(latitude, longitude, localUserTime);
            // Calculate the stars
            await Task.Run(() =>
            {
                CalculateStars(starConverter);
            });

            // Calculate the Messier Objects
            await Task.Run(() =>
            {
                CalculateMessierObjects(converter);

            });

            // Calculate the constellation stars
            await Task.Run(() =>
            {
                CalculateConstellationStars(starConverter);
            });

  
            var planets = new CosineKittyPlanetaryCalculator (latitude, longitude, localUserTime).CalculatePlanets();
            var moon = new CosineKittyMoonCalculator(latitude, longitude, localUserTime).CalculateMoon();

            // Pack up the data and ship it back.
            return new CelestialDataPackage<T>(horizontalStars, horizontalMessierObjects,constellations, ConstellationStars, new ConcurrentDictionary<int, T>(), planets, moon);
            
        }

        private void CalculateConstellationStars(IEquatorialConverter<HorizontalStar> starConverter)
        {
            ConstellationStars.Clear();
            foreach (var item in equatorialConstellationStars)
            {
                var star = starConverter.Convert(item.Value);
                star.StarId = item.Value.StarId;
                star.StarName = item.Value.ProperName;
                star.AbsoluteMagnitude = item.Value.AbsoluteMagnitude;
                star.ColorIndex = item.Value.ColorIndex;
                star.Spectrum = item.Value.Spectrum;
                //Console.WriteLine($"HIP: {item.Value.HipparcosId}");
                star.HipparcosId = item.Value.HipparcosId;
                
                ConstellationStars.TryAdd(star.HipparcosId ?? -1, star);
            }
        }

        private void CalculateMessierObjects(IEquatorialConverter<HorizontalMessierObject> converter)
        {
            List<HorizontalMessierObject> newMessier = new();
            foreach (var item in equatorialMessierObjects)
            {
                var messier = converter.Convert(item);
                messier.MessierId = item.MessierId;
                messier.Size = item.Size;
                messier.ViewingDifficulty = item.ViewingDifficulty;
                messier.Constellation = item.Constellation;
                messier.NewGeneralCatalog = item.NewGeneralCatalog;
                messier.ViewingSeason = item.ViewingSeason;
                newMessier.Add(messier);
            }
            horizontalMessierObjects = newMessier;
            Debug.Print(Value);
        }

        private void CalculateStars(IEquatorialConverter<HorizontalStar> starConverter)
        {
            List<HorizontalStar> newStars = new();
            foreach (var item in equatorialStars)
            {
                var star = starConverter.Convert(item);
                star.StarId = item.StarId;
                star.StarName = item.ProperName;
                star.AbsoluteMagnitude = item.AbsoluteMagnitude;
                star.ColorIndex = item.ColorIndex;
                star.Spectrum = item.Spectrum;
                star.HipparcosId = item.HipparcosId;
                //Console.WriteLine($"Non-Constell HIP: {star.HipparcosId}");

                newStars.Add(star);
            }
            horizontalStars = newStars;
        }
    }
}
          
