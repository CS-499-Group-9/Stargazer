
using DataLayer.HorizontalObjects;
using DataLayer.EquatorialObjects;
using System.Reflection.Metadata;

namespace DataLayer.Interfaces
{
 
    /// <summary>
    /// Converts a <see cref="HorizontalBody"/> to a {T} object
    /// </summary>
    /// <typeparam name="T">The converted value of type {T} (defined by the front end)</typeparam>
    public interface IEquatorialCalculator<T> where T : HorizontalBody
    {
        /// <summary>
        /// The current internal universal time used for calculations.
        /// </summary>
        public DateTime CurrentTime { get;}
       
        /// <summary>
        /// Calculates and updates the position of a <see cref="HorizontalBody"/>
        /// </summary>
        /// <param name="hoBody">The body to perform the calculation on.</param>
        void UpdatePositionOf(T hoBody);

        /// <summary>
        /// Increments the time of the internal universal time used to perform calculations.
        /// </summary>
        /// <param name="seconds"></param>
        void IncrementTimeBy(double seconds);
        void SetTime(DateTime userTime);
    }
}
