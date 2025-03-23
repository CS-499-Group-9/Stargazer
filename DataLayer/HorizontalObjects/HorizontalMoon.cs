using DataLayer.EquatorialObjects;
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
    public class HorizontalMoon : HorizontalBody
    {
        public HorizontalMoon(EquatorialCelestialBody body) : base(body)
        {
        }

        public HorizontalMoon() : base(null)
        {

        }

        /// <summary>
        /// A double representing the moon phase from 0° to 360°
        /// </summary>
        public double Phase { get; internal set; } 


    }
}
