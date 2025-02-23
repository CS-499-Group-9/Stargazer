using DataLayer.EquitorialObjects;

namespace DataLayer.Interfaces
{
    public interface IStarRepository
    {
        Task<IList<EquitorialStar>> GetAllStarsAsync(double maximumMagnitude);
    }
}
