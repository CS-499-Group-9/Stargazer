using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using DataLayer.EquitorialObjects;
using DataLayer.Implementations;
using System.Security.Cryptography.X509Certificates;

namespace DataLayer
{
    public class StargazerRepositoryService<T>
    {
        const double maxStarMagnitude = 6;

        private readonly IEnumerable<EquitorialStar> equitorialStars;
        private readonly IEnumerable<Constellation> constellations;


        private readonly IReadOnlyDictionary<int, EquitorialStar> equitorialConstellationStars;
        private readonly IEnumerable<EquitorialMessierObject> equitorialMessierObjects;

        private readonly BlockingCollection<HorizontalStar> horizontalStars;
        private readonly BlockingCollection<HorizontalConstellation> horizontalConstellations;
        private readonly BlockingCollection<HorizontalMessierObject> horizontalMessierObjects;
        private ConcurrentDictionary<int, HorizontalStar> constellationStars { get;  }

        private StargazerRepositoryService(
            IEnumerable<EquitorialStar> equitorialStars,
            IEnumerable<Constellation> equitorialConstellations,
            IReadOnlyDictionary<int, EquitorialStar> equitorialConstellationStars,
            IEnumerable<EquitorialMessierObject> equitorialMessierObjects)
        {
            this.equitorialStars = equitorialStars;
            this.constellations = equitorialConstellations;
            this.equitorialConstellationStars = equitorialConstellationStars;
            this.equitorialMessierObjects = equitorialMessierObjects;

            horizontalStars = new BlockingCollection<HorizontalStar>(new ConcurrentBag<HorizontalStar>());
            horizontalConstellations = new BlockingCollection<HorizontalConstellation>(new ConcurrentBag<HorizontalConstellation>());
            horizontalMessierObjects = new BlockingCollection<HorizontalMessierObject>(new ConcurrentBag<HorizontalMessierObject>());
            constellationStars = new ConcurrentDictionary<int, HorizontalStar>();
        }

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

            var equitorialStars = getStars.Result;
            var equitorialConstellations = getConstellations.Result;
            var equitorialMessierObjects = getMessierObjects.Result;

            var equitorialConstellationStars = await GatherConstellationStars(getStars, getConstellations, starRepository);

            // Return the fully constructed object
            return new StargazerRepositoryService<T>(
                equitorialStars,
                equitorialConstellations,
                new Dictionary<int, EquitorialStar>(equitorialConstellationStars),
                equitorialMessierObjects
            );
        }

        private static async Task<IDictionary<int, EquitorialStar>> GatherConstellationStars(
            Task<IList<EquitorialStar>> starTask,
            Task<IList<Constellation>> constellationTask,
            IStarRepository starRepository)
        {
            IDictionary<int, EquitorialStar> constellationStars = new Dictionary<int, EquitorialStar>();
            await Task.WhenAll(starTask, constellationTask);

            IList<EquitorialStar> stars = starTask.Result;
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
                                EquitorialStar? star;
                                if (!constellationStars.TryGetValue(endPoint, out star))
                                {
                                    star = stars.FirstOrDefault(s => s.HipparcosId == endPoint);
                                    if (star == null)
                                    { 
                                        star = await starRepository.GetStaryByHipAsync(endPoint);
                                        if (star == null) throw new InvalidOperationException($"{endPoint} was not found.");
                                    }
                                    constellationStars.TryAdd(endPoint, star);
                                    stars.Remove(star);
                                }

                            }
                            catch (InvalidOperationException e)
                            {
                                throw new InvalidOperationException($"Test");

                            }
                        }
                    }
                }
            });

            return constellationStars;
        }

        public CelestialDataPackage<T> CalculateHorizontalObjects(double latitude, double longitude, DateTime localUserTime)
        {
            CosineKittyEquitorialConverter<HorizontalStar> starConverter = new CosineKittyEquitorialConverter<HorizontalStar>(latitude, longitude, localUserTime);
            Task.Factory.StartNew(() =>
            {
                foreach (var item in equitorialStars)
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

            Task.Factory.StartNew(() =>
            {
                CosineKittyEquitorialConverter<HorizontalMessierObject> converter = new CosineKittyEquitorialConverter<HorizontalMessierObject>(latitude, longitude, localUserTime);
                foreach (var item in equitorialMessierObjects)
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

            Task.Factory.StartNew(() =>
            {
                foreach (var item in equitorialConstellationStars)
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
          
