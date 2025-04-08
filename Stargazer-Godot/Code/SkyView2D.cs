using DataLayer;
using Godot;
using System;
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
        private RichTextLabel timeLabel;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Stars = GetNode<Spawner2D>("Stars2D");
            Constellations = GetNode<Constellations2D>("Constellations2D");
            timeLabel = GetNode<RichTextLabel>("TimeLabel2D");
        }

        /// <summary>
        /// This will need to be changed for the new implementation of containerization.
        /// </summary>
        /// <param name="dataPackage"></param>
        /// <returns></returns>
        public async Task UpdateUserPosition(CelestialDataPackage<Star> dataPackage, DateTime currentTime, (double, double) latLong)
        {
            timeLabel.Text = currentTime.ToString() + "\n" + latLong.ToString();
            var drawnStars = await Stars.DrawStars(dataPackage.DrawnStars);
            Constellations.DrawConstellations(dataPackage.Constellations, drawnStars);
        }
    }
}
