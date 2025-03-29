using CosineKitty;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;

namespace DataLayer.Implementations
{
    /// <summary>
    /// Uses the <see cref="CosineKitty.Astronomy"/> library to calculate the position and phase of the moon. 
    /// </summary>
    internal class CosineKittyMoonCalculator : IMoonCalculator
    {
        private readonly Observer observer;
        private DateTime currentTime;
        private AstroTime astroTime;

        /// <summary>
        /// Creates a new calculator
        /// </summary>
        /// <param name="latitude">The latitude of the observer</param>
        /// <param name="longitude">The longitude of the observer</param>
        /// <param name="universalTime">The UTC time of the observer</param>
        public CosineKittyMoonCalculator(double latitude, double longitude, DateTime universalTime)
        {
            currentTime = universalTime;
            observer = new Observer(latitude, longitude, 150);
            astroTime = new AstroTime(universalTime);
        }

        /// <summary>
        /// Instantiates a new <see cref="HorizontalMoon"/> object.
        /// </summary>
        /// <returns>The instantiated moon.</returns>
        public HorizontalMoon CalculateMoon()
        {
            Equatorial equ = Astronomy.Equator(Body.Moon, astroTime, observer, EquatorEpoch.OfDate, Aberration.Corrected);
            Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
            var illumination = Astronomy.Illumination(Body.Moon, astroTime);
            var phase = Astronomy.MoonPhase(astroTime);
            var eqBody = new EquatorialStar { Declination = equ.dec, RightAscension = equ.ra, Distance = equ.dist, Magnitude = illumination.mag };
            return new HorizontalMoon(eqBody);
        }

        /// <inheritdoc/>
        public void UpdatePositionOf(HorizontalMoon moon)
        {
            Equatorial equ = Astronomy.Equator(Body.Moon, astroTime, observer, EquatorEpoch.OfDate, Aberration.Corrected);
            Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
            var illumination = Astronomy.Illumination(Body.Moon, astroTime);
            var phase = Astronomy.MoonPhase(astroTime);
            moon.Azimuth = hor.azimuth;
            moon.Altitude = hor.altitude;
            moon.Phase = phase;
        }

        /// <inheritdoc/>
        public void IncrementTimeBy(double seconds)
        {
            currentTime = currentTime.AddSeconds(seconds);
            astroTime = new(currentTime);
        }

        public void SetTime(DateTime currentTime)
        {
            this.currentTime = currentTime;
        }
    }
}
