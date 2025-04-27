using DataLayer.EquatorialObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.HorizontalObjects
{
    /// <summary>
    /// Represents the Sun in horizontal coordinate form.
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    public class HorizontalSun : HorizontalBody
    {
        /// <summary>
        /// Creates a new instance using a celestial body to contain basic information
        /// </summary>
        /// <param name="body">A new <c>HorizontalSun</c> object.</param>
        public HorizontalSun(EquatorialCelestialBody body) : base(body)
        {
        }
    }
}
