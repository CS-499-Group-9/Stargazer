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
        public IEquatorialCalculator Calculator { get; }
        


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
        
        public HorizontalMoon Moon { get; }

        public Tuple<double, double> DistanceRange { get; }
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
        /// <param name="constellations">A <see cref="List{Constellation}"/>s in Horizontal Coordinate form. </param>
        /// <param name="drawnStars">A <see cref="IDictionary{Int32, T}"/> that have already been drawn in the GUI.</param>
        /// <param name="planets">A <see cref="List{HorizonalPlanet}"/> in the solar system from the users perspective.</param>
        /// <param name="moon">A <see cref="HorizontalMoon"/>The moon object to draw.</param>
        internal CelestialDataPackage(
            IEnumerable<HorizontalStar> stars,
            IEquatorialCalculator starCalculator,
            IEnumerable<HorizontalMessierObject> messierObjects,
            IEnumerable<Constellation> constellations,
            ConcurrentDictionary<int, T> drawnStars,
            IEnumerable<HorizontalPlanet>? planets,
            HorizontalMoon moon)
        {
            horizontalStars = stars;
            Calculator = starCalculator;
            MessierObjects = messierObjects;
            Constellations = constellations;
            this.DrawnStars = drawnStars;
            Planets = planets;
            Moon = moon;

            var min = stars.Min(s => s.Distance);
            var max = stars.Max(s => s.Distance);
        }
    };
}
