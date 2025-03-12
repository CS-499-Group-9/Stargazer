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
    /// Uses the <see cref="CosineKitty.Astronomy"/> library to calculate the position and magnitude of the planets.
    /// </summary>
    internal class CosineKittyPlanetaryCalculator : IPlanetaryCalculator<HorizonalPlanet> 
    {
        private readonly IEnumerable<Body> _bodies = new List<Body> { Body.Mercury, Body.Venus, Body.Mars, Body.Jupiter, Body.Saturn, Body.Uranus, Body.Neptune};
        private readonly Observer observer;
        private readonly AstroTime astroTime;

        /// <summary>
        /// Used to perform the calculations for a given user's position and universal time.
        /// </summary>
        /// <param name="latitude">The latitude of the user.</param>
        /// <param name="longitude">The longitude of the user.</param>
        /// <param name="universalTime">The user's time in universtal time code form.</param>
        public CosineKittyPlanetaryCalculator(double latitude, double longitude, DateTime universalTime)
        {
            observer = new Observer(latitude, longitude, 150);
            astroTime = new AstroTime(universalTime);
        }

        /// <summary>
        /// Performs the calculations for all planets.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{HorizonalPlanet}"/> in horizontal coordinate form.</returns>
        public IEnumerable<HorizonalPlanet> CalculatePlanets()
        {
            List<HorizonalPlanet> planets = new();
            foreach (Body body in _bodies)
            {

                Equatorial equ = Astronomy.Equator(body, astroTime, observer, EquatorEpoch.OfDate, Aberration.Corrected);
                Topocentric hor = Astronomy.Horizon(astroTime, observer, equ.ra, equ.dec, Refraction.Normal);
                var illumination = Astronomy.Illumination(body, astroTime);
                planets.Add(new HorizonalPlanet(body.ToString(),hor.altitude, hor.azimuth, illumination.mag));
            }
            return planets;
        }
    }
}
