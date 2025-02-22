using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.RawObjects
{
    public class RawStar : RawCelestialBody
    {
        internal int StarId { get; set; }
        internal int? HipparcosId { get; set; }
        internal int? HenryDraperId { get; set; }
        internal int? HarvardRevisedId { get; set; }
        internal string? GlieseId { get; set; }
        internal string? BayerFlamsteedDesignation { get; set; }
        internal string? ProperName { get; set; }
        internal double Distance { get; set; }
        internal double AbsoluteMagnitude { get; set; }
        internal string? Spectrum { get; set; }
        internal double? ColorIndex { get; set; }
        internal bool InConstellation { get; set; }
    }
}
