using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using System.Collections.Concurrent;

namespace DataLayer
{

    /// <summary>
    /// Encapsulates all data returned in the Horizontal Coordinate system from the <c>DataLayer</c> needed by the front end to display astronomical objects.
    /// </summary>
    /// <typeparam name="T">The class type of star that is used in the database</typeparam>
    public class CelestialDataPackage<T>
    {

        // These two collections handle the logic of the GetConstellationStar method.
        private IEnumerable<HorizontalStar> horizontalStars;
        /// <summary>
        /// A dictionary containing the stars that have been drawn in the GUI.
        /// </summary>
        public ConcurrentDictionary<int, T> DrawnStars { get; }

        /// <summary>
        /// Gets a read only list of stars to be filtered and drawn.
        /// </summary>
        public IEnumerable<HorizontalStar> HorizontalStars { get { return horizontalStars; } }

        /// <summary>
        /// The calculator used to determine the position of the stars.
        /// </summary>
        public IEquatorialCalculator<HorizontalStar> StarCalculator { get; }
        /// <summary>
        /// The calculator used to determine the position of the Messier Deep Space Objects
        /// </summary>
        public IEquatorialCalculator<HorizontalMessierObject> MessierCalculator { get; }


        /// <summary>
        /// A collection of Messier Deep Space Objects in the Horizontal Coordinate form to be displayed.
        /// </summary>
        public IEnumerable<HorizontalMessierObject> MessierObjects { get; }
        /// <summary>
        /// A collection of Constellations to be displayed.
        /// </summary>
        public IEnumerable<Constellation> Constellations { get; }
        /// <summary>
        /// A collection of solar planets in horizontal coordinate form.
        /// </summary>
        public IEnumerable<HorizontalPlanet>? Planets { get; }
        /// <summary>
        /// The calculator used to determine planetary data.
        /// </summary>
        public IPlanetaryCalculator<HorizontalPlanet> PlanetaryCalculator { get; }
        /// <summary>
        /// The calculator used to determine moon data.
        /// </summary>
        public IMoonCalculator MoonCalculator { get; }
        /// <summary>
        /// The moon in horizontal coordinate form
        /// </summary>
        public HorizontalMoon Moon { get; }

        /// <summary>
        /// Searches the dictionary for a <c>HorizontalStar</c> using the Hip ID provided. If found, converts it to a T star, stores it and returns it.
        /// If not found, retrieves it from the dictionary of stars already converted and returns it.
        /// </summary>
        /// <param name="hipId">The Hipparcos Id of the star</param>
        /// <param name="SpawnStar">The function provided to spawn a new star in the graphic scene. Must take a <c>HorizontalStar</c> as and argument and return a <c>T</c> star</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">If the star was not found.</exception>
        public T GetStar(int hipId, Func<HorizontalStar, T> SpawnStar)
        {
            if (DrawnStars.TryGetValue(hipId, out var star)) return star;
            var horizontalStar = horizontalStars.First(s => s.HipparcosId == hipId);
            if (horizontalStar != null)
            {
                var newStar = SpawnStar(horizontalStar);
                DrawnStars.TryAdd(hipId, newStar);
                return newStar;
            }
            else throw new KeyNotFoundException($"Star with HipparcosId {hipId} not found!");
        }

        /// <summary>
        /// Used by the <see cref="StargazerRepositoryService{T}"/> to pack up the data and return it.
        /// All values are from the user's perspective.
        /// </summary>
        /// <param name="stars">A <see cref="List{HorizontalStar}"/> to be drawn.</param>
        /// <param name="starCalculator">The star calculator for the current lat/long.</param>
        /// <param name="messierObjects">A <see cref="List{HorizontalMessierObject}"/> to be drawn.</param>
        /// <param name="messierCalculator">The Messier Object calculator for the current lat/long.</param>
        /// <param name="constellations">A <see cref="List{Constellation}"/>s in Horizontal Coordinate form. </param>
        /// <param name="drawnStars">A <see cref="IDictionary{Int32, T}"/> that have already been drawn in the GUI.</param>
        /// <param name="planets">A <see cref="List{HorizonalPlanet}"/> in the solar system from the users perspective.</param>
        /// <param name="planetCalculator">The Planet calculator for the current lat/long.</param>
        /// <param name="moon">A <see cref="HorizontalMoon"/>The moon object to draw.</param>
        /// <param name="moonCalculator">The moon calculator for the current lat/long.</param>
        internal CelestialDataPackage(
            IEnumerable<HorizontalStar> stars,
            IEquatorialCalculator<HorizontalStar> starCalculator,
            IEnumerable<HorizontalMessierObject> messierObjects,
            IEquatorialCalculator<HorizontalMessierObject> messierCalculator,
            IEnumerable<Constellation> constellations,
            ConcurrentDictionary<int, T> drawnStars,
            IEnumerable<HorizontalPlanet>? planets,
            IPlanetaryCalculator<HorizontalPlanet> planetCalculator,
            HorizontalMoon moon,
            IMoonCalculator moonCalculator)
        {
            horizontalStars = stars;
            StarCalculator = starCalculator;
            MessierObjects = messierObjects;
            MessierCalculator = messierCalculator;
            Constellations = constellations;
            this.DrawnStars = drawnStars;
            Planets = planets;
            this.PlanetaryCalculator = planetCalculator;
            Moon = moon;
            MoonCalculator = moonCalculator;
        }
    };
}
