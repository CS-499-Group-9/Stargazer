
using DataLayer.EquatorialObjects;

namespace DataLayer.HorizontalObjects
{
    /// <summary>
    /// Represents an object that can be located in the sky according to horizontal coordinates.
    /// </summary>
    public abstract class HorizontalBody
    {
        /// <summary>
        /// Creates a new object by wrapping a <see cref="EquatorialCelestialBody"/>
        /// </summary>
        /// <param name="body"></param>
        protected HorizontalBody(EquatorialCelestialBody body)
        {
            EquatorialBody = body;
        }

        /// <summary>
        /// The <see cref="EquatorialCelestialBody"/> to base off of.
        /// </summary>
        public EquatorialCelestialBody EquatorialBody { get; set; }

        /// <summary>
        /// The angle in decimal degrees formed between the horizon and the star
        /// </summary>
        public double Altitude { get; internal set; }
        /// <summary>
        /// The angle in decimal degrees formed between due north and the star
        /// </summary>
        public double Azimuth { get; internal set; }

        /// <summary>
        /// The apparent brightness of the star
        /// </summary>
        public double Magnitude { get { return EquatorialBody.Magnitude; } internal set { EquatorialBody.Magnitude = value; } }

        /// <summary>
        /// The distance (in lightyears) from the star to earth
        /// </summary>
        public double Distance { get { return EquatorialBody.Distance; } }

    }
}
