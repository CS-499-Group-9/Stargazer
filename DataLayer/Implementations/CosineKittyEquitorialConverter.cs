using CosineKitty;
using DataLayer.HorizonalObjects;
using DataLayer.Interfaces;
using DataLayer.EquitorialObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Implementations
{
    internal class CosineKittyEquitorialConverter<T> : IEquitorialConverter<T> where T : HorizonalBody , new()
    {
        private AstroTime astroTime;
        private Observer observer;

        public CosineKittyEquitorialConverter(double latitude, double longitude, DateTime localUserTime)
        {
            observer = new Observer(latitude, longitude, 150);
            astroTime = new AstroTime(localUserTime);
        }

        public Func<EquitorialCelestialBody, T> Converter => (eqStar) => 
        {
            
            Astronomy.DefineStar(Body.Star1, eqStar.RightAscention, eqStar.Declination, eqStar.Distance);
            Equatorial eq = Astronomy.Equator(Body.Star1, astroTime, observer, EquatorEpoch.J2000, Aberration.Corrected);
            Topocentric hor = Astronomy.Horizon(astroTime, observer, eq.ra, eq.dec, Refraction.None);

            T newBody = new T();
            newBody.Altitude = hor.altitude;
            newBody.Azimuth = hor.azimuth;
            newBody.Magnitude = eqStar.Magnitude;
            newBody.Distance = eqStar.Distance;
            return newBody;
        };


    }
}
