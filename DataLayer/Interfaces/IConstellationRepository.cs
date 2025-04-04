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
        /// <returns>An <see cref="IEnumerable{Constellation}"/> containing graphs of all the constellations.</returns>
        public IEnumerable<Constellation> GetConstellations();
    }
}
