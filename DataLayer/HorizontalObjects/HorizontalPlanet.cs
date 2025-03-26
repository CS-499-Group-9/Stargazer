using DataLayer.EquatorialObjects;
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
    public class HorizontalPlanet : HorizontalBody
    {
        /// <summary>
        /// The common name of the planet.
        /// </summary>
        public string Name { get; set; }
        public double PhaseAngle { get; set; }
        
        /// <summary>
        /// Returns a new planet object
        /// </summary>
        /// <param name="name">The name of the planet</param>
        public HorizontalPlanet(string name, double phaseAngle, EquatorialCelestialBody body) : base(body)
        {
            Name = name;
            PhaseAngle = phaseAngle;
        }


        

    }
}
