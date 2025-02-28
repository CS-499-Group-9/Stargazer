using DataLayer.EquatorialObjects;

namespace DataLayer.Interfaces
{
    public interface IMessierRepository
    {
        Task<IEnumerable<EquatorialMessierObject>> GetRawMessierObjectsAsync();
    }
}
