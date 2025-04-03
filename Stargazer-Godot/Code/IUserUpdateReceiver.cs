using DataLayer;
using Stargazer;
using System.Threading.Tasks;

public interface IUserUpdateReceiver
{
    public Task InitializeCelestial(CelestialDataPackage<Star> dataPackage);
}
