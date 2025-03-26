using DataLayer;
using Godot;
using Stargazer;
using System;
using System.Threading.Tasks;

namespace Stargazer
{
    /// <summary>
    /// Can be used to indicate that a class is capable of receiving the <see cref="Startup.UserPositionUpdated"/> notification.
    /// Is not currently used due to only one class implementing this and then handling individual dispersal of information.
    /// </summary>
    public interface IUserUpdateReceiver
    {
        /// <summary>
        /// Receives the notification
        /// </summary>
        /// <param name="dataPackage">The <see cref="CelestialDataPackage{Star}"/> containing the data to be drawn</param>
        /// <returns>A Task that can be awaited.</returns>
        public Task UpdateUserPosition(CelestialDataPackage<Star> dataPackage);
    }
}