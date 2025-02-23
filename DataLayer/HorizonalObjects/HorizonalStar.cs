using CosineKitty;
using DataLayer.Interfaces;
using DataLayer.EquitorialObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.HorizonalObjects
{
    public class HorizonalStar : HorizonalBody
    {
        public int StarId { get; }
        public string? StarName { get; }
        public double? AbsoluteMagnitude { get; }
        public string? Spectrum { get; }
        public double? ColorIndex { get; }
        

        public HorizonalStar(double altitude, double azimuth, EquitorialStar equitorialStar) : 
            base(altitude, azimuth, equitorialStar)
        {
            StarId = equitorialStar.StarId;
            StarName = equitorialStar.ProperName;
            AbsoluteMagnitude = equitorialStar.AbsoluteMagnitude;
            Spectrum = equitorialStar.Spectrum;
            ColorIndex = equitorialStar.ColorIndex;
        }

    }
}
