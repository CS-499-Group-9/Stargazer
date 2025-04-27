using DataLayer.EquatorialObjects;

namespace DataLayer.HorizontalObjects
{
    /// <summary>
    /// Represents the moon in horizontal coordinate form.
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    public class HorizontalMoon : HorizontalBody
    {
        /// <summary>
        /// Creates a new object by wrapping a <see cref="EquatorialCelestialBody"/>
        /// </summary>
        /// <param name="body">The <see cref="EquatorialCelestialBody"/> to base this object off of.</param>
        public HorizontalMoon(EquatorialCelestialBody body) : base(body)
        {
        }
        /// <summary>
        /// A double representing the moon phase from 0° to 360°
        /// </summary>
        public double Phase { get; internal set; }


    }
}
