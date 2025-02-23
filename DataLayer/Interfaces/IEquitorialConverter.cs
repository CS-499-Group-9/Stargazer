
using CosineKitty;
using DataLayer.HorizonalObjects;
using DataLayer.EquitorialObjects;

namespace DataLayer.Interfaces
{
    public interface IEquitorialConverter<T> where T : HorizonalBody, new()
    {

        Func<EquitorialCelestialBody, T> Converter { get; }
    }
}
