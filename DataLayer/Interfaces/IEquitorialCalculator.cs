
using DataLayer.HorizontalObjects;

namespace DataLayer.Interfaces
{

    /// <summary>
    /// Converts a <see cref="HorizontalBody"/> to a {T} object
    /// </summary>
    /// <typeparam name="T">The converted value of type {T} (defined by the front end)</typeparam>
    public interface IEquatorialCalculator
    {
        /// <summary>
        /// The current internal universal time used for calculations.
        /// </summary>
        public DateTime CurrentTime { get; }
        /// <summary>
        /// The current local sidereal time of the calculator.
        /// </summary>
        double LST { get; }
        /// <summary>
        /// The current latitude of the observer.
        /// </summary>
        double Latitude { get; }
        /// <summary>
        /// The current longitude of the observer.
        /// </summary>
        double Longitude { get; }
        /// <summary>
        /// Creates a new moon object.
        /// </summary>
        /// <returns>A new <see cref="HorizontalMoon"/></returns>
        HorizontalMoon CreateMoon();
        /// <summary>
        /// Creates a new sun object.
        /// </summary>
        /// <returns>A new <see cref="HorizontalSun"/>.</returns>
        HorizontalSun CreateSun();
        /// <summary>
        /// Creates a collection of planets.
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{HorizontalPlanet}"/></returns>
        IEnumerable<HorizontalPlanet> CreatePlanets();

        /// <summary>
        /// Sets the internal time of the calculator.
        /// </summary>
        /// <param name="userTime">User requested time in UTC.</param>
        void SetTime(DateTime userTime);
        /// <summary>
        /// Updates the position of the observer.
        /// </summary>
        /// <param name="latitude">The user's latitude.</param>
        /// <param name="longitude">The user's longitude.</param>
        void SetLocation(double latitude, double longitude);
        /// <summary>
        /// Increments the time of the internal universal time used to perform calculations.
        /// </summary>
        /// <param name="seconds"></param>
        void IncrementTimeBy(double seconds);
        /// <summary>
        /// Calculates and updates the position of a <see cref="HorizontalBody"/>
        /// </summary>
        /// <param name="hoBody">The body to perform the calculation on.</param>
        void UpdatePositionOf(HorizontalBody hoBody);
        /// <summary>
        /// Overloaded method to calculate the position of the moon
        /// </summary>
        /// <param name="moon">The <c>Moon</c> object to perform the calculation on.</param>
        void UpdatePositionOf(HorizontalMoon moon);
        /// <summary>
        /// Overloaded method to calculate the position of a planet.
        /// </summary>
        /// <param name="planet"></param>
        void UpdatePositionOf(HorizontalPlanet planet);
        /// <summary>
        /// Overloaded method to calculate the position of the Sun.
        /// </summary>
        /// <param name="sun"></param>
        void UpdatePositionOf(HorizontalSun sun);

        DateTime getTime()     
        {     
            return CurrentTime;     
        }

        (string, string) getLongLat()
        {
            string latDir = Latitude >= 0 ? "N" : "S";
            string lonDir = Longitude >= 0 ? "E" : "W";
            string latStr = $"{Math.Abs(Math.Round(Latitude, 2))}°{latDir}";
            string lonStr = $"{Math.Abs(Math.Round(Longitude, 2))}°{lonDir}";
            return (latStr, lonStr);
        }
    }
}
