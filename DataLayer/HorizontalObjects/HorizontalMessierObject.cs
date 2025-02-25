using DataLayer.EquitorialObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.HorizontalObjects
{
    public class HorizontalMessierObject : HorizontalBody
    {
        public string? MessierId { get; internal set; }
        public string? NewGeneralCatalog { get ; internal set; }
        public string? Type { get; internal set; }
        public string? Constellation { get; internal set; }
        public string? Size { get; internal set; }
        public string? ViewingSeason { get; internal set; }
        public string? ViewingDifficulty { get; internal set; }
    }
}
