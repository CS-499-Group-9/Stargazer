using CosineKitty;
using DataLayer.HorizontalObjects;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Used to perform planetary calculations
    /// </summary>
    /// <typeparam name="T">The class type to perform the calculations on.</typeparam>
    public interface IPlanetaryCalculator<T> where T : HorizontalBody
    {
        /// <summary>
        /// Initializes the list of planets.
        /// </summary>
        /// <returns>A list of planets</returns>
        public IEnumerable<T> CalculatePlanets();

        /// <summary>
        /// Increments the time of the internal universal time used for calculations
        /// </summary>
        /// <param name="seconds">The number of seconds to increment the time.</param>
        public void IncrementTimeBy(double seconds);
        void SetTime(DateTime currentTime);

        
        public double GetLST();

        /// <summary>
        /// Performs calculations for and updates a planet.
        /// </summary>
        /// <param name="planet">The planet to update.</param>
        public void UpdatePositionOf(HorizontalPlanet planet);

    }
}
