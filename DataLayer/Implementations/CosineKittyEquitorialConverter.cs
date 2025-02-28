using CosineKitty;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using DataLayer.EquatorialObjects;

namespace DataLayer.Implementations
{
    internal class CosineKittyEquatorialConverter<T> : IEquatorialConverter<T> where T : HorizontalBody , new()
    {
        private readonly AstroTime astroTime;
        private readonly Observer observer;

        public CosineKittyEquatorialConverter(double latitude, double longitude, DateTime localUserTime)
        {
            observer = new Observer(latitude, longitude, 150);
            astroTime = new AstroTime(localUserTime);
        }

        public Func<EquatorialCelestialBody, T> Converter => (eqStar) => 
        {
            
            Astronomy.DefineStar(Body.Star1, eqStar.RightAscension, eqStar.Declination, eqStar.Distance);
            Equatorial eq = Astronomy.Equator(Body.Star1, astroTime, observer, EquatorEpoch.J2000, Aberration.Corrected);
            Topocentric hor = Astronomy.Horizon(astroTime, observer, eq.ra, eq.dec, Refraction.None);

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
