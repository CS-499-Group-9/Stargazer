using System.Collections.Concurrent;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using DataLayer.EquatorialObjects;
using DataLayer.Implementations;

namespace DataLayer
{
    /// <summary>
    /// Used to make requests to convert data from the equatorial coordinate system to the horizontal coordinate system and receive a data package containing all values.
    /// </summary>
    /// <typeparam name="T">The class type of the star to be drawn by the front end. 
    /// This is needed for the internal logic of the <see cref="CelestialDataPackage{T}"/></typeparam>
    public class StargazerRepositoryService<T>
    {
        const double maxStarMagnitude = 6;

        private readonly IEnumerable<EquatorialStar> equatorialStars;
        private readonly IEnumerable<Constellation> constellations;


        private readonly IReadOnlyDictionary<int, EquatorialStar> equatorialConstellationStars;
        private readonly IEnumerable<EquatorialMessierObject> equatorialMessierObjects;

        private readonly BlockingCollection<HorizontalStar> horizontalStars;
        private readonly BlockingCollection<HorizontalMessierObject> horizontalMessierObjects;
        private ConcurrentDictionary<int, HorizontalStar> constellationStars { get;  }

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

            horizontalStars = new BlockingCollection<HorizontalStar>(new ConcurrentBag<HorizontalStar>());
            horizontalMessierObjects = new BlockingCollection<HorizontalMessierObject>(new ConcurrentBag<HorizontalMessierObject>());
            constellationStars = new ConcurrentDictionary<int, HorizontalStar>();
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
            var getStars = starRepository.GetAllStarsAsync(maxStarMagnitude);
            var getMessierObjects = messierRepository.GetRawMessierObjectsAsync();
            var getConstellations = constellationRepository.GetAllConstellationsAsync();

            // Wait for all data to be fetched asynchronously
            await Task.WhenAll(getStars, getConstellations, getMessierObjects);

            var equatorialStars = getStars.Result;
            var equatorialConstellations = getConstellations.Result;
            var equatorialMessierObjects = getMessierObjects.Result;

            var equatorialConstellationStars = await GatherConstellationStars(getStars, getConstellations, starRepository);

            // Return the fully constructed object
            return new StargazerRepositoryService<T>(
                equatorialStars,
                equatorialConstellations,
                new Dictionary<int, EquatorialStar>(equatorialConstellationStars),
                equatorialMessierObjects
            );
        }

        private static async Task<IDictionary<int, EquatorialStar>> GatherConstellationStars(
            Task<IList<EquatorialStar>> starTask,
            Task<IList<Constellation>> constellationTask,
            IStarRepository starRepository)
        {
            IDictionary<int, EquatorialStar> constellationStars = new Dictionary<int, EquatorialStar>();
            await Task.WhenAll(starTask, constellationTask);

            IList<EquatorialStar> stars = starTask.Result;
            IList<Constellation> constellations = constellationTask.Result;

            await Task.Run(async () =>
            {
                foreach (var constellation in constellations)
                {
                    foreach (var line in constellation.ConstellationLines)
                    {
                        var endPoints = new int[] { line.Item1, line.Item2 };
                        foreach (var endPoint in endPoints)
                        {
                            try
                            {
                                if (!constellationStars.TryGetValue(endPoint, out EquatorialStar? star))
                                {
                                    star = stars.FirstOrDefault(s => s.HipparcosId == endPoint);
                                    if (star == null)
                                    {
                                        star = await starRepository.GetStarByHipAsync(endPoint);
                                        if (star == null) throw new InvalidOperationException($"{endPoint} was not found.");
                                    }
                                    constellationStars.TryAdd(endPoint, star);
                                    stars.Remove(star);
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
            CosineKittyEquatorialConverter<HorizontalStar> starConverter = new(latitude, longitude, localUserTime);
            await  Task.Factory.StartNew(() =>
            {
                foreach (var item in equatorialStars)
                {
                    var star = starConverter.Converter(item);
                    star.StarId = item.StarId;
                    star.StarName = item.ProperName;
                    star.AbsoluteMagnitude = item.AbsoluteMagnitude;
                    star.ColorIndex = item.ColorIndex;
                    star.Spectrum = item.Spectrum;

                    horizontalStars.TryAdd(star);
                }
                horizontalStars.CompleteAdding();
            });

            await  Task.Factory.StartNew(() =>
            {
                CosineKittyEquatorialConverter<HorizontalMessierObject> converter = new(latitude, longitude, localUserTime);
                foreach (var item in equatorialMessierObjects)
                {
                    var messier = converter.Converter(item);
                    messier.MessierId = item.MessierId;
                    messier.Size = item.Size;
                    messier.ViewingDifficulty = item.ViewingDifficulty;
                    messier.Constellation = item.Constellation;
                    messier.NewGeneralCatalog = item.NewGeneralCatalog;
                    messier.ViewingSeason = item.ViewingSeason;
                    horizontalMessierObjects.TryAdd(messier);
                }
                horizontalMessierObjects.CompleteAdding();

            });

            await Task.Factory.StartNew(() =>
            {
                foreach (var item in equatorialConstellationStars)
                {
                    var star = starConverter.Converter(item.Value);
                    star.StarId = item.Value.StarId;
                    star.StarName = item.Value.ProperName;
                    star.AbsoluteMagnitude = item.Value.AbsoluteMagnitude;
                    star.ColorIndex = item.Value.ColorIndex;
                    star.Spectrum = item.Value.Spectrum;
                    star.HipparcosId = item.Value.HipparcosId;
                    constellationStars.TryAdd(star.HipparcosId ?? -1, star);
                }
            });

            return new CelestialDataPackage<T>(horizontalStars, horizontalMessierObjects,constellations, constellationStars, new ConcurrentDictionary<int, T>());
            
        }
    }
}
          
