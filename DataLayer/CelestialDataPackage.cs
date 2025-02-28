using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using System.Collections.Concurrent;

namespace DataLayer
{
    public record CelestialDataPackage<T>
    (
        BlockingCollection<HorizontalStar> Stars, 
        BlockingCollection<HorizontalMessierObject> MessierObjects, 
        IEnumerable<Constellation> Constellations, 
        ConcurrentDictionary<int, HorizontalStar> ConstellationStars, 
        ConcurrentDictionary<int, T> DrawnStars
    );
}
