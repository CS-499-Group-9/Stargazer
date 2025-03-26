using DataLayer.HorizontalObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Used to calculate the altitude and azimuths of planets.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPlanetaryCalculator<T> where T : HorizontalBody
    {
        /// <summary>
        /// Used to initially create the planets
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> CreatePlanets();

        /// <summary>
        /// Increments the internal universal time of the calculator.
        /// </summary>
        /// <param name="increment"></param>
        public void IncrementTimeBy(double increment);

        /// <summary>
        /// Updates the position of the passed planet using the current internal universal time.
        /// </summary>
        /// <param name="planet">The planet to perform the calculations on.</param>
        public void UpdatePositionOf(HorizontalPlanet planet);
    }
}
