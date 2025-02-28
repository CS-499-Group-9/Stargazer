using DataLayer.EquatorialObjects;

namespace DataLayer.Interfaces
{
    public interface IStarRepository
    {
        public Task<IList<EquatorialStar>> GetAllStarsAsync(double maximumMagnitude);
        public Task<EquatorialStar?> GetStarByHipAsync(int hipparcosId);
    }
}
