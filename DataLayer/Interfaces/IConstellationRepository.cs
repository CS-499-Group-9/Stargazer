using DataLayer.EquitorialObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IConstellationRepository
    {
        internal Task<IList<Constellation>> GetAllConstellationsAsync();
    }
}
