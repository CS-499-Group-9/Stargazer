
using DataLayer.EquatorialObjects;

namespace DataLayer.HorizontalObjects
{
    /// <summary>
    /// A Messier Deep Space Object located in Horizontal coordinates
    /// </summary>
    public class HorizontalMessierObject : HorizontalBody
    {
        public HorizontalMessierObject(EquatorialMessierObject body) : base(body)
        {
            MessierId = body.MessierId;
            NewGeneralCatalog = body.NewGeneralCatalog;
            Type = body.Type;
            Constellation = body.Constellation;
            Size = body.Size;
            ViewingDifficulty = body.ViewingDifficulty;
            ViewingSeason = body.ViewingSeason;
        }

        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialMessierObject.MessierId"/>
        /// </summary>
        public string? MessierId { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialMessierObject.NewGeneralCatalog"/>
        /// </summary>
        public string? NewGeneralCatalog { get ; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialMessierObject.Type"/>
        /// </summary>
        public string? Type { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialMessierObject.Constellation"/>
        /// </summary>
        public string? Constellation { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialMessierObject.Size"/>
        /// </summary>
        public string? Size { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialMessierObject.ViewingSeason"/>
        /// </summary>
        public string? ViewingSeason { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialMessierObject.ViewingDifficulty"/>
        /// </summary>
        public string? ViewingDifficulty { get; internal set; }
    }
}
