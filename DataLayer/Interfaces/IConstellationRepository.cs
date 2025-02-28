using DataLayer.EquatorialObjects;

namespace DataLayer.Interfaces
{
    public interface IConstellationRepository
    {
        internal Task<IList<Constellation>> GetAllConstellationsAsync();
    }
}
