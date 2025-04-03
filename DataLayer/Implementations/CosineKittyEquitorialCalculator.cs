using CosineKitty;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using System.Security;

namespace DataLayer.Implementations
{
    /// <summary>
    /// Converts an object of type <see cref="EquatorialCelestialBody"/> from the equatorial coordinate system to horizontal coordinates using the CosineKitty.AstronomyEngine library
    /// </summary>
    /// <typeparam name="T">The type of <see cref="HorizontalBody"/> to convert to</typeparam>
    internal class CosineKittyEquatorialCalculator : IEquatorialCalculator
    {
        private DateTime currentTime;
        private AstroTime astroTime;
        private Observer observer;
        private readonly Dictionary<string, Body> planets;
        private const double AUConversion = 63241.0771;

        public double LST { get { return Astronomy.SiderealTime(astroTime); } }

        /// <summary>
        /// Generates a new converter specific to the observers location and time
        /// </summary>
        /// <param name="latitude">The latitude of the observer</param>
        /// <param name="longitude">The longitude of the observer</param>
        /// <param name="universalTime">The Universal Coordinated Time</param>
        internal CosineKittyEquatorialCalculator(double latitude, double longitude, DateTime universalTime)
        {
            observer = new Observer(latitude, longitude, 150);
            currentTime = universalTime;
            astroTime = new AstroTime(universalTime);
            planets = new Dictionary<string, Body>
            {
                {Body.Mercury.ToString(), Body.Mercury },
                { Body.Venus.ToString(), Body.Venus },
                { Body.Mars.ToString(), Body.Mars },
                { Body.Jupiter.ToString(), Body.Jupiter },
                { Body.Saturn.ToString(), Body.Saturn },
                {Body.Uranus.ToString(), Body.Uranus },
                {Body.Neptune.ToString(), Body.Neptune },
                {Body.Sun.ToString(), Body.Sun}
            };
            observer = new Observer(latitude, longitude, 150);
            currentTime = universalTime;
            astroTime = new AstroTime(universalTime);
        }

        internal CosineKittyEquatorialCalculator() : this(0, 0, new AstroTime(2000, 1, 1, 12, 0, 0).ToUtcDateTime())
        {
        }

        /// <summary>
        /// Retrieves the internal universal time used for calculations.
        /// </summary>
        public DateTime CurrentTime { get { return currentTime; } }

        public void SetTime(DateTime userTime)
        {
            currentTime = userTime;
            astroTime = new AstroTime(userTime);
        }

        /// <inheritdoc/>
        public void IncrementTimeBy(double seconds)
        {
            currentTime = currentTime.AddSeconds(seconds);
            astroTime = new(currentTime);
        }
        /// <inheritdoc/>
       
        public void UpdatePositionOf(HorizontalBody hoBody)
        {
            var eqBody = hoBody.EquatorialBody;
            Astronomy.DefineStar(Body.Star1, eqBody.RightAscension, eqBody.Declination, eqBody.Distance >= 1 ? eqBody.Distance : 1);
            Equatorial eq = Astronomy.Equator(Body.Star1, astroTime, observer, EquatorEpoch.J2000, Aberration.Corrected);
            // Determine that stars horizontal coordinates
            Topocentric hor = Astronomy.Horizon(astroTime, observer, eq.ra, eq.dec, Refraction.None);
            hoBody.Altitude = hor.altitude;
            hoBody.Azimuth = hor.azimuth;
            hoBody.Distance = eq.dist/AUConversion;
        }

        ///<inheritdoc/>
        public void UpdatePositionOf(HorizontalPlanet planet)
        {
            if (planets.TryGetValue(planet.Name, out var body))
            {
                Equatorial equ = Astronomy.Equator(body, astroTime, observer, EquatorEpoch.J2000, Aberration.Corrected);
                //var eqBody = new EquatorialStar { ProperName = body.ToString(), RightAscension = equ.ra, Declination = equ.dec, Distance = equ.dist };
                Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
                var illumination = Astronomy.Illumination(body, astroTime);
                planet.Azimuth = hor.azimuth;
                planet.Altitude = hor.altitude;
                planet.PhaseAngle = illumination.phase_angle;
                planet.Distance = equ.dist/AUConversion;
            }
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
            moon.Distance = equ.dist / AUConversion;
        }

        /// <summary>
        /// Performs the calculations for all planets.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{HorizonalPlanet}"/> in horizontal coordinate form.</returns>
        public IEnumerable<HorizontalPlanet> CreatePlanets()
        {
            List<HorizontalPlanet> planetList = new();
            foreach (var body in planets)
            {
                Equatorial equ = Astronomy.Equator(body.Value, astroTime, observer, EquatorEpoch.J2000, Aberration.Corrected);
                var eqBody = new EquatorialStar
                {
                    ProperName = body.ToString(),
                    RightAscension = equ.ra,
                    Declination = equ.dec,
                    Distance = equ.dist
                };
                //Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
                var illumination = Astronomy.Illumination(body.Value, astroTime);
                planetList.Add(new HorizontalPlanet(body.Key, illumination.phase_angle, eqBody));
            }
            return planetList;
        }

        /// <summary>
        /// Instantiates a new <see cref="HorizontalMoon"/> object.
        /// </summary>
        /// <returns>The instantiated moon.</returns>
        public HorizontalMoon CreateMoon()
        {
            Equatorial equ = Astronomy.Equator(Body.Moon, astroTime, observer, EquatorEpoch.OfDate, Aberration.Corrected);
            Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
            var illumination = Astronomy.Illumination(Body.Moon, astroTime);
            var phase = Astronomy.MoonPhase(astroTime);
            var eqBody = new EquatorialStar { Declination = equ.dec, RightAscension = equ.ra, Distance = equ.dist, Magnitude = illumination.mag };
            return new HorizontalMoon(eqBody);
        }

        public void SetLocation(double latitude, double longitude)
        {
            observer = new(latitude, longitude, 150);
        }
    }
}
