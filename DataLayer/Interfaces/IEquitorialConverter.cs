
using CosineKitty;
using DataLayer.HorizontalObjects;
using DataLayer.EquitorialObjects;

namespace DataLayer.Interfaces
{
    public interface IEquitorialConverter<T> where T : HorizontalBody, new()
    {

        Func<EquitorialCelestialBody, T> Converter { get; }
    }
}
