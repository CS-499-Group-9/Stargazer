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
    internal class CosineKittyPlanetaryCalculator : IPlanetaryCalculator<HorizonalPlanet> 
    {
        private readonly IEnumerable<Body> _bodies = new List<Body> { Body.Mercury, Body.Venus, Body.Mars, Body.Jupiter, Body.Saturn, Body.Uranus, Body.Neptune};
        private readonly Observer observer;
        private readonly AstroTime astroTime;

        public CosineKittyPlanetaryCalculator(double latitude, double longitude, DateTime universalTime)
        {
            observer = new Observer(latitude, longitude, 150);
            astroTime = new AstroTime(universalTime);
        }

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
