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
    internal class CosineKittyEquatorialConverter<T> : IEquatorialConverter<T> where T : HorizontalBody , new()
    {
        private readonly AstroTime astroTime;
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
            astroTime = new AstroTime(universalTime);
        }

        /// <summary>
        /// Performs the conversion and returns the <c>T</c> object of type <see cref="HorizontalBody"/>
        /// </summary>
        public Func<EquatorialCelestialBody, T> Convert => (eqStar) => 
        {
            // Define a new star
            Astronomy.DefineStar(Body.Star1, eqStar.RightAscension, eqStar.Declination, eqStar.Distance);
            // Place that star in the equatorial coordinate system for the observers location and time
            Equatorial eq = Astronomy.Equator(Body.Star1, astroTime, observer, EquatorEpoch.J2000, Aberration.Corrected);
            // Determine that stars horizontal coordinates
            Topocentric hor = Astronomy.Horizon(astroTime, observer, eq.ra, eq.dec, Refraction.None);

            // Create the new object.
            T newBody = new()
            {
                Altitude = hor.altitude,
                Azimuth = hor.azimuth,
                Magnitude = eqStar.Magnitude,
                Distance = eqStar.Distance
            };
            return newBody;
        };


    }
}
