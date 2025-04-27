using DataLayer.EquatorialObjects;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Retrieves a list of Messier Deep Space Objects from a repository
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    public interface IMessierRepository
    {
        /// <summary>
        /// Gets all Messier Objects from the repository
        /// </summary>
        /// <returns>A <c>yieldable</c> <see cref="IEnumerable{EquatorialMessierObject}"/> that can be lazily loaded when needed.</returns>
        public IEnumerable<EquatorialMessierObject> GetMessierObjects();
    }
}
