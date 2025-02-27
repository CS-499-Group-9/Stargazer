using DataLayer.EquitorialObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.HorizontalObjects
{
    public abstract class HorizontalBody
    {

        public double Altitude { get; internal set; }
        public double Azimuth { get; internal set; }
        public double Magnitude { get; internal set; }
        public double Distance { get; internal set; } 

       

    }
}
