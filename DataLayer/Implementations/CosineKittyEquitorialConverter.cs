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
    internal class CosineKittyEquatorialConverter<T> : IEquatorialCalculator<T> where T : HorizontalBody
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
        /// Retrieves the internal universal time used for calculations.
        /// </summary>
        public DateTime CurrentTime { get { return currentTime; } }

        /// <inheritdoc/>
        public void IncrementTimeBy(double seconds) 
        { 
            currentTime = currentTime.AddSeconds(seconds);
            astroTime = new(currentTime); 
        }

        /// <inheritdoc/>
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

        public void SetTime(DateTime userTime)
        {
            currentTime = userTime;
        }
    }
}
