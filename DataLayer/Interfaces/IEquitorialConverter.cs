
using DataLayer.HorizontalObjects;
using DataLayer.EquatorialObjects;

namespace DataLayer.Interfaces
{
    public interface IEquatorialConverter<T> where T : HorizontalBody, new()
    {

        Func<EquatorialCelestialBody, T> Converter { get; }
    }
}
