using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
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
        private  IDictionary<int, HorizontalStar> ConstellationStars { get;  }
        private IDictionary<int, T> DrawnConstellationStars { get; }


        /// <summary>
        /// A collection of stars in the Horizontal Coordinate form to be displayed.
        /// </summary>
        public IEnumerable<HorizontalStar> Stars { get; }

        /// <summary>
        /// A collection of Messier Deep Space Objects in the Horizontal Coordinate form to be displayed.
        /// </summary>
        public IEnumerable<HorizontalMessierObject> MessierObjects { get;  }
        /// <summary>
        /// A collection of Constellations to be displayed.
        /// </summary>
        public  IEnumerable<Constellation> Constellations { get;  }
        /// <summary>
        /// A collection of solar planets in horizontal coordinate form.
        /// </summary>
        public IEnumerable<HorizonalPlanet>? Planets { get; }
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
        /// <exception cref="KeyNotFoundException"></exception>
        public T GetConstellationStar(int hipId, Func<HorizontalStar, T> SpawnStar)
        {
            if (ConstellationStars.Remove(hipId, out var horizontalStar))
            {
                var newStar = SpawnStar(horizontalStar);
                DrawnConstellationStars.TryAdd(hipId, newStar);
                return newStar;
            }
            if (!DrawnConstellationStars.TryGetValue(hipId, out var star))  throw new KeyNotFoundException();
            return star;
        }

        /// <summary>
        /// Used by the <see cref="StargazerRepositoryService{T}"/> to pack up the data and return it.
        /// All values are from the user's perspective.
        /// </summary>
        /// <param name="stars">A <see cref="List{HorizontalStar}"/> to be drawn.</param>
        /// <param name="messierObjects">A <see cref="List{HorizontalMessierObject}"/> to be drawn.</param>
        /// <param name="constellations">A <see cref="List{Constellation}"/>s in Horizontal Coordinate form. </param>
        /// <param name="constellationStars">A <see cref="IDictionary{Int32, HorizontalStar}"/> that have not yet been drawn in the GUI.</param>
        /// <param name="drawnStars">A <see cref="IDictionary{Int32, T}"/> that have already been drawn in the GUI.</param>
        /// <param name="planets">A <see cref="List{HorizonalPlanet}"/> in the solar system from the users perspective.</param>
        /// <param name="moon">A <see cref="HorizontalMoon"/>.</param>
        internal CelestialDataPackage(
            IEnumerable<HorizontalStar> stars, 
            IEnumerable<HorizontalMessierObject> messierObjects, 
            IEnumerable<Constellation> constellations, 
            ConcurrentDictionary<int, HorizontalStar> constellationStars, 
            ConcurrentDictionary<int, T> drawnStars,
            IEnumerable<HorizonalPlanet>? planets,
            HorizontalMoon moon)
        {
            Stars = stars;
            MessierObjects = messierObjects;
            Constellations = constellations;
            ConstellationStars = constellationStars;
            DrawnConstellationStars = drawnStars;
            Planets = planets;
            Moon = moon;
        }
    };
}
