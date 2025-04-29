using DataLayer.EquatorialObjects;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Communicates with a repository of Yale Stars (in the equatorial coordinate form).
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    public interface IStarRepository
    {
        /// <summary>
        /// Gets all stars from the repository. (Omit the sun, so that it can be created separately).
        /// </summary>
        /// <returns>A <c>yieldable</c> <see cref="IEnumerable{EquatorialStar}"/> that can is lazily loaded during read.</returns>
        public IEnumerable<EquatorialStar> GetAllStars();

    }
}
