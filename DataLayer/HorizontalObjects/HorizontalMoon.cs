using DataLayer.EquatorialObjects;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.HorizontalObjects
{
    /// <summary>
    /// Represents the moon in horizontal coordinate form.
    /// </summary>
    public class HorizontalMoon : HorizontalBody, IPhaseable
    {
        private double moonPhase;

        /// <summary>
        /// Creates a new moon
        /// </summary>
        /// <param name="body">The <see cref="EquatorialCelestialBody"/> used to perform calculations.</param>
        public HorizontalMoon(EquatorialCelestialBody body) : base(body)
        {
        }
        
        /// <inheritdoc/>
        public double Phase { get { return moonPhase; } set { moonPhase = value; } }
    }
}
