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

        private CelestialDataPackage<Star> dataPackage;

        private string screenshotPath = "user://screenshot.jpeg";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Stars = GetNode<Spawner2D>("Stars2D");
            Constellations = GetNode<Constellations2D>("Constellations2D");
            GD.Print("SkyView2D created.");
            if (Input.IsActionJustPressed("screenshot_key"))
            {
                GD.Print("skyview exists");
                GD.Print("Screenshot time!");
                TakeScreenshot();
            }
        }


        /// <summary>
        /// This will need to be changed for the new implementation of containerization.
        /// </summary>
        /// <param name="dataPackage"></param>
        /// <returns></returns>
        public async Task UpdateUserPosition(CelestialDataPackage<Star> inputPackage)
        {
            dataPackage = inputPackage;
            
        }
        private async Task TakeScreenshot()
        {
            await Stars.DrawStars(dataPackage.DrawnStars);
            await Constellations.DrawConstellations(dataPackage.Constellations, dataPackage.GetStar);
            GD.Print("Screenshot time!");
            // Get the current viewport as an Image
            Viewport viewport = GetViewport();
            Image screenshotImage = viewport.GetTexture().GetImage();

            // Save the screenshot as a JPEG
            screenshotImage.SavePng(screenshotPath);
            GD.Print($"Screenshot saved to {screenshotPath}");
        }
        public override void _Input(InputEvent @event)
        {


        }
    }
}