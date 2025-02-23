using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.HorizonalObjects;
using DataLayer.Interfaces;
using DataLayer.EquitorialObjects;
using DataLayer.Implementations;

namespace DataLayer
{
    public class StargazerRepositoryService
    {
        const int maxStarMagnitude = 6;

        private readonly IEnumerable<EquitorialStar> equitorialStars;
        private readonly IEnumerable<EquitorialConstellation> equitorialConstellations;
        private readonly IEnumerable<EquitorialMessierObject> equitorialMessierObjects;

        private readonly BlockingCollection<HorizonalStar> convertedStars;
        private readonly BlockingCollection<HorizonalConstellation> convertedConstellations;
        private readonly BlockingCollection<EquitorialMessierObject> convertedMessierObjects;

        public StargazerRepositoryService(IStarRepository starRepository, IConstellationRepository constellationRepository, IMessierRepository messierRepository)
        {
            var getStars = starRepository.GetAllStarsAsync(maxStarMagnitude);
            //var getConstellations = constellationRepository.GetAllConstellationsAsync();
            //var getMessierObjects = messierRepository.GetRawMessierObjectsAsync();

            getStars.Start();
            //getConstellations.Start();
            //getMessierObjects.Start();

            convertedStars = new BlockingCollection<HorizonalStar>(new ConcurrentBag<HorizonalStar>());
            convertedConstellations = new BlockingCollection<HorizonalConstellation>(new ConcurrentBag<HorizonalConstellation>());
            convertedMessierObjects = new BlockingCollection<EquitorialMessierObject>(new ConcurrentBag<EquitorialMessierObject>());

            //Task.WaitAll(getStars, getConstellations, getMessierObjects);
            Task.WaitAll(getStars);
            equitorialStars = getStars.Result;
            equitorialConstellations = new List<EquitorialConstellation>();
            equitorialMessierObjects = new List<EquitorialMessierObject>();
        }

        public void CalculateStars(double longitude, double latitude, DateTime localUserTime)
        {
            Task.Factory.StartNew(() => 
            {
                CosineKittyEquitorialConverter<HorizonalStar> converter = new CosineKittyEquitorialConverter<HorizonalStar>(latitude, longitude, localUserTime);
                foreach (var item in equitorialStars)
                {
                    var star = converter.Converter(item);
                    star.StarId = item.StarId;
                    star.StarName = item.ProperName;
                    star.AbsoluteMagnitude = item.AbsoluteMagnitude;
                    star.ColorIndex = item.ColorIndex;
                    star.Spectrum = item.Spectrum;

                    convertedStars.TryAdd(star);
                }
                convertedStars.CompleteAdding();
            });

            Task.Factory.StartNew(() => 
            {
                foreach (var item in equitorialConstellations)
                {
                    
                }
                
            });

            Task.Factory.StartNew(() => 
            {
                foreach (var item in equitorialMessierObjects)
                {
                    
                }
            });
        }

        public BlockingCollection<HorizonalStar> GetStars() { return convertedStars; }
    }

    
}
