using DataLayer.EquitorialObjects;
using DataLayer.HorizontalObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public record CelestialDataPackage<T>(BlockingCollection<HorizontalStar> Stars, BlockingCollection<HorizontalMessierObject> MessierObjects, IEnumerable<Constellation> Constellations, ConcurrentDictionary<int, HorizontalStar> ConstellationStars, ConcurrentDictionary<int, T> DrawnStars);
}
