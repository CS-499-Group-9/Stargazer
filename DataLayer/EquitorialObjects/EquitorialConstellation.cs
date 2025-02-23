using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EquitorialObjects
{
    internal class EquitorialConstellation : EquitorialCelestialBody
    {
        internal string ConstellationId { get; set; }
        internal IList<int> ConstellationLines { get; set; }

        internal string ConstellationName { get; set; } 


    }
}
