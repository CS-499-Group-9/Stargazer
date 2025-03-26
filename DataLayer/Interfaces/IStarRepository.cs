using DataLayer.EquatorialObjects;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Communicates with a repository of Yale Stars (in the equatorial coordinate form).
    /// </summary>
    public interface IStarRepository
    {
        /// <summary>
        /// Gets all stars that are brighter than a certain magnitude.
        /// </summary>
        /// <returns>A running task that can be awaited to obtain a list of stars</returns>
        public Task<IList<EquatorialStar>> GetAllStarsAsync();

        /// <summary>
        /// Gets a single star from the repository
        /// </summary>
        /// <param name="hipparcosId">The Hipparcos ID of the star to find</param>
        /// <returns>Null if no star is found</returns>
        public Task<EquatorialStar?> GetStarByHipAsync(int hipparcosId);
    }
}
