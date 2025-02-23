using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EquitorialObjects

{
    public abstract class EquitorialCelestialBody
    {
        public double RightAscention { get; set; }
        public double Declination { get; set; }
        public double Magnitude { get; set; }
        public double Distance { get; set; }
    }
}
