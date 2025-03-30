using DataLayer.HorizontalObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Calculates the position of the moon in horizontal form and phase as a <see cref="double"/> between 0° and 360°
    /// </summary>
    public interface IMoonCalculator
    {

        /// <summary>
        /// Performs the moon calculation
        /// </summary>
        /// <returns>A <see cref="HorizontalMoon"/> object.</returns>
        public HorizontalMoon CalculateMoon();

        /// <summary>
        /// Increments the internal universal time used for calculations
        /// </summary>
        /// <param name="seconds"></param>
        public void IncrementTimeBy(double seconds);

        /// <summary>
        /// Updates the position of a <see cref="HorizontalMoon"/>
        /// </summary>
        /// <param name="moon">The moon to update the position of.</param>
        public void UpdatePositionOf(HorizontalMoon moon);
    }
}
