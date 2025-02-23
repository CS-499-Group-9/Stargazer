using DataLayer.EquitorialObjects;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Implementations
{
    internal class StellariumJsonConstellationRepository : IConstellationRepository
    {
        Task<IList<EquitorialConstellation>> IConstellationRepository.GetAllConstellationsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
