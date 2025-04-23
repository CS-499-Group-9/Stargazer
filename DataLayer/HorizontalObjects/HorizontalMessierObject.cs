
using DataLayer.EquatorialObjects;

namespace DataLayer.HorizontalObjects
{
    /// <summary>
    /// A Messier Deep Space Object located in Horizontal coordinates
    /// </summary>
    public class HorizontalMessierObject : HorizontalBody
    {
        /// <summary>
        /// Creates a new object by wrapping a <see cref="EquatorialMessierObject"/>
        /// </summary>
        /// <param name="body">The <see cref="EquatorialMessierObject"/> to base this object off of.</param>
        public HorizontalMessierObject(EquatorialMessierObject body) : base(body)
        {
            MessierId = body.MessierId;
            NewGeneralCatalog = body.NewGeneralCatalog;
            Type = body.Type;
            Constellation = body.Constellation;
            Size = body.Size;
            ViewingDifficulty = body.ViewingDifficulty;
            ViewingSeason = body.ViewingSeason;
            CommonName = body.CommonName;
            Console.WriteLine($"Common name {CommonName}");
        }

        /// <summary>
        /// <see cref="EquatorialMessierObject.MessierId"/>
        /// </summary>
        public string? MessierId { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialMessierObject.NewGeneralCatalog"/>
        /// </summary>
        public string? NewGeneralCatalog { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialMessierObject.Type"/>
        /// </summary>
        public string? Type { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialMessierObject.Constellation"/>
        /// </summary>
        public string? Constellation { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialMessierObject.Size"/>
        /// </summary>
        public string? Size { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialMessierObject.ViewingSeason"/>
        /// </summary>
        public string? ViewingSeason { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialMessierObject.ViewingDifficulty"/>
        /// </summary>
        public string? ViewingDifficulty { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialMessierObject.CommonName"/>
        /// </summary>
        public string? CommonName { get; internal set; }
    }
}
