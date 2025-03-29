using DataLayer;
using Stargazer;
using System.Threading.Tasks;

public interface IUserUpdateReceiver
{
    public Task UpdateUserPosition(CelestialDataPackage<Star> dataPackage);
}
