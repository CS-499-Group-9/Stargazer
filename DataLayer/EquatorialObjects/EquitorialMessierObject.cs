
namespace DataLayer.EquatorialObjects
{
    public class EquatorialMessierObject : EquatorialCelestialBody
    {
        private string? id;
        private string? newGeneralCatalog;
        private string? type;
        private string? constellation;
        private string? size;
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
