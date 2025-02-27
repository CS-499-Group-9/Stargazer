using DataLayer.EquitorialObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.HorizontalObjects
{
    public class HorizontalConstellation
    {
        public IEnumerable<Tuple<int, HorizontalStar>>? Lines { get; internal set; }
    }
}
