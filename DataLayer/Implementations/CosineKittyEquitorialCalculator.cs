using CosineKitty;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using System.Diagnostics;
using System.Security;

namespace DataLayer.Implementations
{
    /// <summary>
    /// Uses the <see cref="CosineKitty.Astronomy"/> library to perform calculations on celestial objects.
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    internal class CosineKittyEquatorialCalculator : IEquatorialCalculator
    {
        private DateTime currentTime;
        private AstroTime astroTime;
        private Observer observer;
        private readonly Dictionary<string, Body> planets;                      // Stores a mapping of the planets to the enumeration used to identify the planet. Uses the planets name as the key. 
        private const double AUConversion = 63241.0771;                         // Astronomical Units per Lightyear
        /// <inheritdoc/>
        public DateTime CurrentTime { get { return currentTime; } }
        /// <inheritdoc/>
        public double LST { get { return Astronomy.SiderealTime(astroTime); } }
        /// <inheritdoc/>
        public double Latitude { get { return observer.latitude; } }
        /// <inheritdoc/>
        public double Longitude { get { return observer.longitude; } }

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

            // Create the dictionary of planets and map their string name to their enumeration.
            planets = new Dictionary<string, Body>
            {
                {Body.Mercury.ToString(), Body.Mercury },
                { Body.Venus.ToString(), Body.Venus },
                { Body.Mars.ToString(), Body.Mars },
                { Body.Jupiter.ToString(), Body.Jupiter },
                { Body.Saturn.ToString(), Body.Saturn },
                {Body.Uranus.ToString(), Body.Uranus },
                {Body.Neptune.ToString(), Body.Neptune }
            };
        }

        /// <summary>
        /// Used to create a calculator for an observer located at the intersection of the equator and the prime meridian at the J2000 Epoc.
        /// </summary>
        internal CosineKittyEquatorialCalculator() : this(0, 0, new AstroTime(2000, 1, 1, 12, 0, 0).ToUtcDateTime())
        {
        }


        /// <inheritdoc/>
        public HorizontalMoon CreateMoon()
        {
            Equatorial equ = Astronomy.Equator(Body.Moon, astroTime, observer, EquatorEpoch.OfDate, Aberration.Corrected);
            Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
            var illumination = Astronomy.Illumination(Body.Moon, astroTime);
            var phase = Astronomy.MoonPhase(astroTime);
            var eqBody = new EquatorialStar { Declination = equ.dec, RightAscension = equ.ra, Distance = equ.dist, Magnitude = illumination.mag };
            return new HorizontalMoon(eqBody);
        }
        /// <inheritdoc/>
        public HorizontalSun CreateSun()
        {
            Equatorial equ = Astronomy.Equator(Body.Sun, astroTime, observer, EquatorEpoch.OfDate, Aberration.Corrected);
            Topocentric hor = Astronomy.Horizon(astroTime,observer,equ.ra, equ.dec,Refraction.Normal);
            var illumination = Astronomy.Illumination(Body.Sun, astroTime);
            var eqBody = new EquatorialStar{ Declination = equ.dec, RightAscension= equ.ra, Distance = equ.dist, Magnitude= illumination.mag };
            return new HorizontalSun(eqBody);
        }

        /// <inheritdoc/>
        public IEnumerable<HorizontalPlanet> CreatePlanets()
        {
            List<HorizontalPlanet> planetList = new();
            
            // Loop through the dictionary created in the constructor and create a HorizontalPlanet object for each of them.
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
                var illumination = Astronomy.Illumination(body.Value, astroTime);
                // The name is provided and used as they key to find the enumeration in the GetPositionOf method.
                planetList.Add(new HorizontalPlanet(body.Key, illumination.phase_angle, eqBody));
            }
            return planetList;
        }


        ///<inheritdoc/>
        public void SetTime(DateTime userTime)
        {
            currentTime = userTime;
            astroTime = new AstroTime(userTime);
        }

        /// <inheritdoc/>
        public void SetLocation(double latitude, double longitude)
        {
            observer = new(latitude, longitude, 150);
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
                planet.Distance = equ.dist;
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
            moon.Distance = equ.dist;
        }
        /// <inheritdoc/>
        public void UpdatePositionOf(HorizontalSun sun)
        {
            Equatorial equ = Astronomy.Equator(Body.Sun, astroTime, observer, EquatorEpoch.OfDate, Aberration.Corrected);
            Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
            var illumination = Astronomy.Illumination(Body.Sun, astroTime);
            sun.Azimuth = hor.azimuth;
            sun.Altitude = hor.altitude;
            sun.Distance = equ.dist; 
        }

    }
}
