
using DataLayer.ConvertedObjects;
using DataLayer.RawObjects;

namespace DataLayer.Interfaces
{
    public interface IEquitorialConverter
    {
        public Func<RawCelestialBody, CelestialBody> Converter { get; }
    }
}
