using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.RawObjects
{
    public abstract class RawCelestialBody
    {
        internal double RightAscention { get; set; }
        internal double Declination { get; set; }
        internal double Magnitude { get; set; }


    }
}
