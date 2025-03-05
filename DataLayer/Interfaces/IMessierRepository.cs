using DataLayer.EquatorialObjects;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Retrieves a list of Messier Deep Space Objects from a repository
    /// </summary>
    public interface IMessierRepository
    {
        /// <summary>
        /// Gets all Messier Objects from the repository
        /// </summary>
        /// <returns>A running task that can be awaited.</returns>
        Task<IEnumerable<EquatorialMessierObject>> GetRawMessierObjectsAsync();
    }
}
