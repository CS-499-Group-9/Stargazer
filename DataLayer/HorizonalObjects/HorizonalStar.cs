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
        public int StarId { get; internal set; }
        public string? StarName { get; internal set; }
        public double? AbsoluteMagnitude { get; internal set; }
        public string? Spectrum { get; internal set; }
        public double? ColorIndex { get; internal set; }
        



    }
}
