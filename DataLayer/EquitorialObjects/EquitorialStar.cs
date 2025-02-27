using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EquitorialObjects
{
    public class EquitorialStar : EquitorialCelestialBody
    {
        public int StarId { get; set; }
        public int? HipparcosId { get; set; }
        public int? HenryDraperId { get; set; }
        public int? HarvardRevisedId { get; set; }
        public string? GlieseId { get; set; }
        public string? BayerFlamsteedDesignation { get; set; }
        public string? ProperName { get; set; }
        public double AbsoluteMagnitude { get; set; }
        public string? Spectrum { get; set; }
        public double? ColorIndex { get; set; }
        public bool InConstellation { get; set; }
    }
}
