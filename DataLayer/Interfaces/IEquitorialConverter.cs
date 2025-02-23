
using CosineKitty;
using DataLayer.HorizonalObjects;
using DataLayer.EquitorialObjects;

namespace DataLayer.Interfaces
{
    public interface IEquitorialConverter
    {

        Func<EquitorialCelestialBody, HorizonalBody> Converter { get; }
    }
}
