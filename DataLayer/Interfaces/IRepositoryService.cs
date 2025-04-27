namespace DataLayer.Interfaces
{
    /// <summary>
    /// Used to retrieve the <see cref="CelestialDataPackage{T}"/>.
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    /// <typeparam name="T">The repository service.</typeparam>
    public interface IRepositoryService<T>
    {
        /// <summary>
        /// Used to actually pull the data from the repository and build a <see cref="CelestialDataPackage{T}"/>
        /// This is only required to be called once. From there, the <see cref="CelestialDataPackage{T}"/> contains an <see cref="IEquatorialCalculator"/> that handles all future modifications of data.
        /// </summary>
        /// <returns>A <see cref="Task{CelestialDataPackage}"/> that can be awaited.</returns>
        public Task<CelestialDataPackage<T>> InitializeDataPackage();
    }
}