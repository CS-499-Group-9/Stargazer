using CosineKitty;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using DataLayer.EquatorialObjects;

namespace DataLayer.Implementations
{
    /// <summary>
    /// Converts an object of type <see cref="EquatorialCelestialBody"/> from the equatorial coordinate system to horizontal coordinates using the CosineKitty.AstronomyEngine library
    /// </summary>
    /// <typeparam name="T">The type of <see cref="HorizontalBody"/> to convert to</typeparam>
    internal class CosineKittyEquatorialConverter<T> : IEquatorialConverter<T> where T : HorizontalBody
    {
        private DateTime currentTime;
        private AstroTime astroTime;
        private readonly Observer observer;

        /// <summary>
        /// Generates a new converter specific to the observers location and time
        /// </summary>
        /// <param name="latitude">The latitude of the observer</param>
        /// <param name="longitude">The longitude of the observer</param>
        /// <param name="universalTime">The Universal Coordinated Time</param>
        internal CosineKittyEquatorialConverter(double latitude, double longitude, DateTime universalTime)
        {
            observer = new Observer(latitude, longitude, 150);
            currentTime = universalTime;
            astroTime = new AstroTime(universalTime);
        }

        /// <summary>
        /// Gets the current universal time used to perform calculations.
        /// </summary>
        public DateTime CurrentTime { get { return currentTime; } }

        /// <summary>
        /// Increments the internal universal time by seconds
        /// </summary>
        /// <param name="seconds">The number of seconds to increment the universal time.</param>
        public void IncrementTimeBy(double seconds) 
        { 
            currentTime = currentTime.AddSeconds(seconds);
            astroTime = new(currentTime); 
        }

        /// <summary>
        /// Updates the altitude and azimuth of the passed <see cref="HorizontalBody"/>
        /// </summary>
        /// <param name="hoBody">The <see cref="HorizontalBody"/> to perform the calculation on.</param>
        public void UpdatePositionOf(T hoBody)
        {
            var eqBody = hoBody.EquatorialBody;
            Astronomy.DefineStar(Body.Star1, eqBody.RightAscension, eqBody.Declination, eqBody.Distance);
            Equatorial eq = Astronomy.Equator(Body.Star1, astroTime, observer, EquatorEpoch.J2000, Aberration.Corrected);
            // Determine that stars horizontal coordinates
            Topocentric hor = Astronomy.Horizon(astroTime, observer, eq.ra, eq.dec, Refraction.None);
            hoBody.Altitude = hor.altitude;
            hoBody.Azimuth = hor.azimuth;
        }
    }
}
