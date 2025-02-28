
namespace DataLayer.HorizontalObjects
{
    public class HorizontalStar : HorizontalBody
    {
        public int StarId { get; internal set; }
        public int? HipparcosId { get; internal set; }
        public string? StarName { get; internal set; }
        public double? AbsoluteMagnitude { get; internal set; }
        public string? Spectrum { get; internal set; }
        public double? ColorIndex { get; internal set; }
    }
}
