using DataLayer.EquatorialObjects;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Used to retrieve a collection of <see cref="Constellation"/> from a repository.
    /// </summary>
    public interface IConstellationRepository
    {
        /// <summary>
        /// Gets all <see cref="Constellation"/>s from the repository
        /// </summary>
        /// <returns>A running task that can be awaited</returns>
        public Task<IList<Constellation>> GetAllConstellationsAsync();
    }
}
