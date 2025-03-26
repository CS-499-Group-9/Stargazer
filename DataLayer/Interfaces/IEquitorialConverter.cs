
using DataLayer.HorizontalObjects;
using DataLayer.EquatorialObjects;
using System.Reflection.Metadata;

namespace DataLayer.Interfaces
{
 
    /// <summary>
    /// Converts a <see cref="HorizontalBody"/> to a {T} object
    /// </summary>
    /// <typeparam name="T">The converted value of type {T} (defined by the front end)</typeparam>
    public interface IEquatorialConverter<T> where T : HorizontalBody
    {
        public DateTime CurrentTime { get;}
        /// <summary>
        /// A function accepting an object of type <see cref="EquatorialCelestialBody"/> to type {T}
        /// </summary>

        void UpdatePositionOf(T hoBody);
        void IncrementTimeBy(double seconds);
    }
}
