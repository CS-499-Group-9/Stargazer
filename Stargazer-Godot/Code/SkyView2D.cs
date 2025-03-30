using DataLayer;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;
using System.Threading.Tasks;

namespace Stargazer
{
    /// <summary>
    /// Used to create the 2D star scene (for screenshot export). 
    /// </summary>
    public partial class SkyView2D : Node2D, IUserUpdateReceiver
    {
        [Export] private Spawner2D Stars;
        [Export] private Constellations2D Constellations;



        /// <summary>
        /// This will need to be changed for the new implementation of containerization.
        /// </summary>
        /// <param name="dataPackage"></param>
        /// <returns></returns>
        public async Task UpdateUserPosition(CelestialDataPackage<Star> dataPackage)
        {
          await Stars.DrawStars(dataPackage.DrawnStars);
          await Constellations.DrawConstellations(dataPackage.Constellations, dataPackage.GetStar);
        }
    }
}