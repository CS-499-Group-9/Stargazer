using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.HorizontalObjects
{
    /// <summary>
    /// A planet in our solar system in horizontal coordinate form.
    /// </summary>
    public class HorizonalPlanet : HorizontalBody
    {
        /// <summary>
        /// The common name of the planet.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Returns a new planet object
        /// </summary>
        /// <param name="name">The name of the planet</param>
        /// <param name="altitude">The vertical angle formed with the horizon.</param>
        /// <param name="azimuth">The horizontal angle formed with the north pole.</param>
        /// <param name="magnitude">The apparent magnitude of the planet.</param>
        public HorizonalPlanet(string name, double altitude, double azimuth, double magnitude)
        {
            Name = name;
            Altitude = altitude;
            Azimuth = azimuth;
            Magnitude = magnitude;
        }


        

    }
}
