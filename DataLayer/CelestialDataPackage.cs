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
        private  ConcurrentDictionary<int, HorizontalStar> ConstellationStars { get;  }
        private ConcurrentDictionary<int, T> DrawnStars { get; }


        /// <summary>
        /// A collection of stars in the Horizontal Coordinate form to be displayed.
        /// <code>while(!Stars.IsCompleted)
        /// {
        ///     foreach(var star in Stars.GetConsumingEnumerable()
        ///     {
        ///         //Convert and display
        ///     }
        /// }
        /// </code>
        /// </summary>
        public BlockingCollection<HorizontalStar> Stars { get; }

        /// <summary>
        /// A collection of Messier Deep Space Objects in the Horizontal Coordinate form to be displayed.
        /// <code>while(!MessierObjects.IsCompleted)
        /// {
        ///     foreach(var star in MessierObjects.GetConsumingEnumerable()
        ///     {
        ///         //Convert and display
        ///     }
        /// }
        /// </code>
        /// </summary>
        public BlockingCollection<HorizontalMessierObject> MessierObjects { get;  }
        /// <summary>
        /// A collection of Constellations to be displayed.
        /// </summary>
        public  IEnumerable<Constellation> Constellations { get;  }

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
            HorizontalStar horizontalStar;
            if (ConstellationStars.TryRemove(hipId, out horizontalStar))
            {
                var newStar = SpawnStar(horizontalStar);
                DrawnStars.TryAdd(hipId, newStar);
                return newStar;
            }
            var star = DrawnStars.GetValueOrDefault(hipId) ?? throw new KeyNotFoundException();
            return star;
        }
            
        
        internal CelestialDataPackage(BlockingCollection<HorizontalStar> stars, BlockingCollection<HorizontalMessierObject> messierObjects, IEnumerable<Constellation> constellations, ConcurrentDictionary<int, HorizontalStar> constellationStars, ConcurrentDictionary<int, T> drawnStars)
        {
            Stars = stars;
            MessierObjects = messierObjects;
            Constellations = constellations;
            ConstellationStars = constellationStars;
            DrawnStars = drawnStars;
        }
    };
}
