using DataLayer.EquitorialObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.HorizonalObjects
{
    public class HorizonalMessierObject : HorizonalBody
    {
        public HorizonalMessierObject(double altitude, double azimuth, EquitorialCelestialBody equitorialBody) : base(altitude, azimuth, equitorialBody)
        {
        }
    }
}
