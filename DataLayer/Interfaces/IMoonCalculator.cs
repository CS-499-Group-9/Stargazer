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
        public HorizontalMoon CreateMoon();

        public void IncrementTimeBy(double increment);

        public void UpdatePositionOf(HorizontalMoon moon);
    }
}
