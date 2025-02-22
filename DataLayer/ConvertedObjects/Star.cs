using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.ConvertedObjects
{
    internal class Star : CelestialBody
    {

        internal const double parsecToLightyear = 3.262;
        public Star(double altitude, double azimuth, double magnitude) : base(altitude, azimuth, magnitude > 0 ? magnitude * parsecToLightyear : -1)
        {
        }
    }
}
