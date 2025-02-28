
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
