using DataLayer.EquitorialObjects;

namespace DataLayer.Interfaces
{
    public interface IStarRepository
    {
        public Task<IList<EquitorialStar>> GetAllStarsAsync(double maximumMagnitude);
        public Task<EquitorialStar?> GetStaryByHipAsync(int hipparcosId);
    }
}
