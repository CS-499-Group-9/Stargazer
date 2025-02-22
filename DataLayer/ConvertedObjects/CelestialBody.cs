using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.ConvertedObjects
{
    public abstract class CelestialBody
    {
        // In degrees
        internal double altitude;
        // In degrees
        internal double azimuth;
        // In lightyears (may need to be converted)
        internal double magnitude;

        public CelestialBody(double altitude, double azimuth, double magnitude)
        {
            this.altitude = altitude;
            this.azimuth = azimuth;
            this.magnitude = magnitude;
        }
    }
}
