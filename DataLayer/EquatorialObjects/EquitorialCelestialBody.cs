
namespace DataLayer.EquatorialObjects

{
    /// <summary>
    /// Represents an object that can be located in the sky according to equatorial coordinates.
    /// </summary>
    public abstract class EquatorialCelestialBody
    {
        /// <summary>
        /// Measured in decimal hours from the point in the sky where the sun crosses the celestial equator during the spring equinox of 2000
        /// </summary>
        public double RightAscension { get; set; }
        /// <summary>
        /// A measure in degrees how far an object is north or south of the celestial equator during the spring equinox of 2000
        /// </summary>
        public double Declination { get; set; }
        /// <summary>
        /// A logarithmic representation of a star's brightness (negative values being brighter)
        /// </summary>
        public double Magnitude { get; set; }
        /// <summary>
        /// Distance in light years from the earth.
        /// </summary>
        public double Distance { get; set; }
    }
}
