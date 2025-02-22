using DataLayer.RawObjects;

namespace DataLayer.Interfaces
{
    public interface IStarRepository
    {
        public Task<IDictionary<int, RawStar>> GetStars(double maximumMagnitude);
    }
}
