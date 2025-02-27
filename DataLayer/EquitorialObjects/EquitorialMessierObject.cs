using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EquitorialObjects
{
    public class EquitorialMessierObject : EquitorialCelestialBody
    {
        private string? id;
        private string? newGeneralCatalog;
        private string? type;
        private string? constellation;
        private string? size;
        private double? distance;
        private string? viewingSeason;
        private string? viewingDifficulty;

        public string? MessierId { get => id; set => id = value; }
        public string? NewGeneralCatalog { get => newGeneralCatalog; set => newGeneralCatalog = value; }
        public string? Type { get => type; set => type = value; }
        public string? Constellation { get => constellation; set => constellation = value; }
        public string? Size { get => size; set => size = value; }
        public string? ViewingSeason { get => viewingSeason; set => viewingSeason = value; }
        public string? ViewingDifficulty { get => viewingDifficulty; set => viewingDifficulty = value; }
    }
}
