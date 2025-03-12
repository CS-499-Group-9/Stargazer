using DataLayer.HorizontalObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    internal interface IMoonCalculator
    {
        public HorizontalMoon CalculateMoon();
    }
}
