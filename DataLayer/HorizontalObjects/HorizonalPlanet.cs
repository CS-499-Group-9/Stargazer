using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.HorizontalObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class HorizonalPlanet : HorizontalBody
    {
        public string Name { get; set; }
        
        public HorizonalPlanet(string name, double altitude, double azimuth, double magnitude)
        {
            Name = name;
            Altitude = altitude;
            Azimuth = azimuth;
            Magnitude = magnitude;
        }


        

    }
}
