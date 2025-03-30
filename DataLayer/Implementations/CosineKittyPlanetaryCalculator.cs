using CosineKitty;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;

namespace DataLayer.Implementations
{
    /// <summary>
    /// Uses the <see cref="CosineKitty.Astronomy"/> library to calculate the position and magnitude of the planets.
    /// </summary>
    internal class CosineKittyPlanetaryCalculator : IPlanetaryCalculator<HorizontalPlanet>
    {
        private readonly IDictionary<string, Body> _bodies;
        private readonly Observer observer;
        private DateTime currentTime;
        private AstroTime astroTime;

        /// <summary>
        /// Used to perform the calculations for a given user's position and universal time.
        /// </summary>
        /// <param name="latitude">The latitude of the user.</param>
        /// <param name="longitude">The longitude of the user.</param>
        /// <param name="universalTime">The user's time in universtal time code form.</param>
        public CosineKittyPlanetaryCalculator(double latitude, double longitude, DateTime universalTime)
        {
            _bodies = new Dictionary<string, Body>
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

        /// <summary>
        /// Performs the calculations for all planets.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{HorizonalPlanet}"/> in horizontal coordinate form.</returns>
        public IEnumerable<HorizontalPlanet> CalculatePlanets()
        {
            List<HorizontalPlanet> planets = new();
            foreach (var body in _bodies)
            {
                Equatorial equ = Astronomy.Equator(body.Value, astroTime, observer, EquatorEpoch.J2000, Aberration.Corrected);
                var eqBody = new EquatorialStar { ProperName = body.ToString(), RightAscension = equ.ra, Declination = equ.dec, Distance = equ.dist };
                //Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
                var illumination = Astronomy.Illumination(body.Value, astroTime);
                planets.Add(new HorizontalPlanet(body.Key, illumination.phase_angle, eqBody));
            }
            return planets;
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
            astroTime = new(currentTime);
        }

        ///<inheritdoc/>
        public void UpdatePositionOf(HorizontalPlanet planet)
        {
            if (_bodies.TryGetValue(planet.Name, out var body))
            {
                Equatorial equ = Astronomy.Equator(body, astroTime, observer, EquatorEpoch.J2000, Aberration.Corrected);
                //var eqBody = new EquatorialStar { ProperName = body.ToString(), RightAscension = equ.ra, Declination = equ.dec, Distance = equ.dist };
                Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
                var illumination = Astronomy.Illumination(body, astroTime);
                planet.Azimuth = hor.azimuth;
                planet.Altitude = hor.altitude;
                planet.PhaseAngle = illumination.phase_angle;
            }
        }
    }
}
