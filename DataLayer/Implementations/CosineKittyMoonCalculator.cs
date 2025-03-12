using CosineKitty;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Implementations
{
    /// <summary>
    /// Uses the <see cref="CosineKitty.Astronomy"/> library to calculate the position and phase of the moon. 
    /// </summary>
    internal class CosineKittyMoonCalculator : IMoonCalculator
    {
        private readonly Observer observer;
        private readonly AstroTime astroTime;

        /// <summary>
        /// Creates a new calculator
        /// </summary>
        /// <param name="latitude">The latitude of the observer</param>
        /// <param name="longitude">The longitude of the observer</param>
        /// <param name="universalTime">The UTC time of the observer</param>
        public CosineKittyMoonCalculator(double latitude, double longitude, DateTime universalTime)
        {
            observer = new Observer(latitude, longitude, 150);
            astroTime = new AstroTime(universalTime);
        }

        public HorizontalMoon CalculateMoon()
        {
            Equatorial equ = Astronomy.Equator(Body.Moon, astroTime, observer, EquatorEpoch.OfDate, Aberration.Corrected);
            Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
            var illumination = Astronomy.Illumination(Body.Moon, astroTime);
            var phase = Astronomy.MoonPhase(astroTime);
            return new HorizontalMoon { Altitude = hor.altitude, Azimuth = hor.azimuth, Phase = phase, Magnitude = illumination.mag };
        }
    }
}
