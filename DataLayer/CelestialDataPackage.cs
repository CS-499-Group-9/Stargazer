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

        public ConcurrentDictionary<int, T> DrawnStars { get; }



        public IEnumerable<HorizontalStar> HorizontalStars { get { return horizontalStars; } }

        public IEquatorialConverter<HorizontalStar> StarConverter { get; }
        public IEquatorialConverter<HorizontalMessierObject> MessierConverter { get; }


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
        public IEnumerable<HorizonalPlanet>? Planets { get; }
        public IPlanetaryCalculator<HorizonalPlanet> PlanetaryCalculator {get;}

        public IMoonCalculator MoonCalculator {get;}
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
        public T GetStar(int hipId, Func<HorizontalStar, T> SpawnStar)
        {
            if (DrawnStars.TryGetValue(hipId, out var star))  return star;
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
        /// <param name="messierObjects">A <see cref="List{HorizontalMessierObject}"/> to be drawn.</param>
        /// <param name="constellations">A <see cref="List{Constellation}"/>s in Horizontal Coordinate form. </param>
        /// <param name="constellationStars">A <see cref="IDictionary{Int32, HorizontalStar}"/> that have not yet been drawn in the GUI.</param>
        /// <param name="drawnStars">A <see cref="IDictionary{Int32, T}"/> that have already been drawn in the GUI.</param>
        /// <param name="planets">A <see cref="List{HorizonalPlanet}"/> in the solar system from the users perspective.</param>
        /// <param name="moon">A <see cref="HorizontalMoon"/>.</param>
        internal CelestialDataPackage(
            IEnumerable<HorizontalStar> stars,
            IEquatorialConverter<HorizontalStar> starConverter,
            IEnumerable<HorizontalMessierObject> messierObjects,
            IEquatorialConverter<HorizontalMessierObject> messierConverter,
            IEnumerable<Constellation> constellations, 
            ConcurrentDictionary<int, T> drawnStars,
            IEnumerable<HorizonalPlanet>? planets,
            IPlanetaryCalculator<HorizonalPlanet> planetCalculator,
            HorizontalMoon moon,
            IMoonCalculator moonCalculator)
        {
            horizontalStars = stars;
            StarConverter = starConverter;
            MessierObjects = messierObjects;
            MessierConverter = messierConverter;
            Constellations = constellations;
            this.DrawnStars = drawnStars;
            Planets = planets;
            this.PlanetaryCalculator = planetCalculator;
            Moon = moon;
            MoonCalculator = moonCalculator;
        }
    };
}
