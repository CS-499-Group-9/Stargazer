
namespace DataLayer.EquatorialObjects
{
   /// <summary>
   /// Represents a constellation in the sky
   /// </summary>   
    public class Constellation
    {
        /// <summary>
        /// The id of the constellation
        /// </summary>
        public string? ConstellationId { get; internal set; }

        /// <summary>
        /// A list of vertices representing edges in the graph (each vertex is the HipparcosId of a star)
        /// </summary>
        public IEnumerable<Tuple<int, int>> ConstellationLines { get; internal set; }

        /// <summary>
        /// The English(native) name of the constellation
        /// </summary>
        public string? ConstellationName { get; internal set; }
        /// <summary>
        /// Returns a new constellation
        /// </summary>
        /// <param name="Id">The identifying name of the constellation</param>
        /// <param name="name">English name of the constellation</param>
        public int? HipparcosId { get; set; }
        /// <param name="nativeName">The native name of the constellation (if known)</param>
        public Constellation(string Id, string name, string? nativeName)
        {
            ConstellationId = Id;
            ConstellationName = string.Equals(name,nativeName) ? name : $"{name} ({nativeName})";
            ConstellationLines = new List<Tuple<int, int>>();
        }

    }
}
