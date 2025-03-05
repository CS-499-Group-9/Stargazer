
namespace DataLayer.EquatorialObjects
{
    /// <summary>
    /// Represents a Messier Deep Space Object located in equatorial coordinates
    /// </summary>
    public class EquatorialMessierObject : EquatorialCelestialBody
    {

        /// <summary>
        /// The 'M' prefixed identification number of the object
        /// </summary>
        public string? MessierId { get ; set ; }
        /// <summary>
        /// The Messier Object identification in the New General Catalog
        /// </summary>
        public string? NewGeneralCatalog { get ; set ; }
        /// <summary>
        /// The Messier Object category
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// The constellation containing the object
        /// </summary>
        public string? Constellation { get; set; }
        /// <summary>
        /// The size (in light years) of the object (a 2 dimensional value as viewed)
        /// </summary>
        public string? Size { get; set; }
        /// <summary>
        /// The celestial season in which the object can be viewed
        /// </summary>
        public string? ViewingSeason { get; set; }
        /// <summary>
        /// The difficulty level of viewing the object
        /// </summary>
        public string? ViewingDifficulty { get; set; }
    }
}
