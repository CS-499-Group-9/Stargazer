using DataLayer;
using Godot;
using System.Threading.Tasks;

namespace Stargazer
{
    /// <summary>
    /// Used to create the 2D star scene (for screenshot export). 
    /// </summary>
    public partial class SkyView2D : Node2D
    {
        private Spawner2D Stars;
        private Constellations2D Constellations;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Stars = GetNode<Spawner2D>("Stars2D");
            Constellations = GetNode<Constellations2D>("Constellations2D");

        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {

        }

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