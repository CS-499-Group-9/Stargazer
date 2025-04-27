using DataLayer;
using Godot;
using System.Threading.Tasks;
using System;

namespace Stargazer
{
    /// <summary>
    /// Used to create the 2D star scene (for screenshot export). 
    /// Author: William Arnett
    /// Created: SPR 2025
    /// Refactored by Josh Johner (SPR 2025) for containerization improvement.
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
        /// Receives the current data package on screenshot request.
        /// </summary>
        /// <param name="dataPackage"></param>
        /// <returns></returns>
        public async Task UpdateUserPosition(CelestialDataPackage<Star> dataPackage, DateTime currentTime, (string, string) latLong)
        {
            timeLabel.Text = currentTime.ToString("M/d/yyyy\nh:mm:ss tt 'UTC'") + "\n" + latLong;
            var drawnStars = await Stars.DrawStars(dataPackage.DrawnStars);
            Constellations.DrawConstellations(dataPackage.Constellations, drawnStars);
        }
    }
}