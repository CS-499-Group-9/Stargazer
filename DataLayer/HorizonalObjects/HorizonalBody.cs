using DataLayer.EquitorialObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.HorizonalObjects
{
    public class HorizonalBody
    {

        public double Altitude { get; }
        public double Azimuth { get; }
        public double Magnitude { get; }
        public double Distance { get; protected set; } 

        public HorizonalBody(double altitude, double azimuth, EquitorialCelestialBody body)
        {
            Altitude = altitude;
            Azimuth = azimuth;
            Magnitude = body.Magnitude;
            Distance = body.Distance;
        }

    }
}
