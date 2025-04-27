
using DataLayer.EquatorialObjects;

namespace DataLayer.HorizontalObjects
{
    /// <summary>
    /// A star located by the Horizontal Coordinate method
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    public class HorizontalStar : HorizontalBody
    {
        private EquatorialStar EquatorialStar;

        /// <summary>
        /// Creates a new object by wrapping an <see cref="EquatorialStar"/>
        /// </summary>
        /// <param name="body">The <see cref="EquatorialStar"/> to base this object off of.</param>
        public HorizontalStar(EquatorialStar body) : base(body)
        {
            EquatorialStar = body;
        }

        /// <summary>
        /// <see cref="EquatorialStar.StarId"/>
        /// </summary>
        public int StarId { get { return EquatorialStar.StarId; } }
        /// <summary>
        /// <see cref="EquatorialStar.HipparcosId"/>
        /// </summary>
        public int? HipparcosId { get { return EquatorialStar.HipparcosId; } }
        /// <summary>
        /// <see cref="EquatorialStar.ProperName"/>
        /// </summary>
        public string? StarName { get { return EquatorialStar.ProperName ?? $"HipId: {EquatorialStar.HipparcosId}"; } }
        /// <summary>
        /// <see cref="EquatorialStar.AbsoluteMagnitude"/>
        /// </summary>
        public double? AbsoluteMagnitude { get { return EquatorialStar.AbsoluteMagnitude; } }
        /// <summary>
        /// <see cref="EquatorialStar.Spectrum"/>
        /// </summary>
        public string? Spectrum { get { return EquatorialStar.Spectrum; } }
        /// <summary>
        /// <see cref="EquatorialStar.ColorIndex"/>
        /// </summary>
        public double? ColorIndex { get { return EquatorialStar.ColorIndex; } }
    }
}
