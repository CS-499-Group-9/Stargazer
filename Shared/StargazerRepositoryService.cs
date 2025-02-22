using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.ConvertedObjects;
using DataLayer.Interfaces;
using DataLayer.RawObjects;

namespace Shared
{
    public class StargazerRepositoryService
    {
        public IDictionary<int, RawStar> celestialBodies;

        public StargazerRepositoryService(IStarRepository starRepository)
        {
            celestialBodies = starRepository.GetStars(6).Result;
        }
    }
}
