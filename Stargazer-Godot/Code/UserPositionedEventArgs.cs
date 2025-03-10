using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGameProject.Code
{
    public class UserPositionedEventArgs : EventArgs
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime DateTime { get; set; }
    }
}
