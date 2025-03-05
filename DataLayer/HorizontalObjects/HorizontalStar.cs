
namespace DataLayer.HorizontalObjects
{
    /// <summary>
    /// A star located by the Horizontal Coordinate method
    /// </summary>
    public class HorizontalStar : HorizontalBody
    {
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialStar.StarId"/>
        /// </summary>
        public int StarId { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialStar.HipparcosId"/>
        /// </summary>
        public int? HipparcosId { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialStar.ProperName"/>
        /// </summary>
        public string? StarName { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialStar.AbsoluteMagnitude"/>
        /// </summary>
        public double? AbsoluteMagnitude { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialStar.Spectrum"/>
        /// </summary>
        public string? Spectrum { get; internal set; }
        /// <summary>
        /// <see cref="EquatorialObjects.EquatorialStar.ColorIndex"/>
        /// </summary>
        public double? ColorIndex { get; internal set; }
    }
}
