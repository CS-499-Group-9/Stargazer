using DataLayer;
using Godot;
using System;
using System.Threading.Tasks;

namespace Stargazer
{
    /// <summary>
    /// The top level controller for the program. 
    /// Handles all communication between the GUI and the viewport.
    /// </summary>
    public partial class Startup : Control
    {
        // Just in case something changes, it's easy to find.
        private const string viewPortPath = "SubViewportContainer/SubViewport/View";
        private StargazerRepositoryService<Star> repositoryService;

        /// <summary>
        /// A <see cref="Delegate"/> used to notify all subscribers that new star data has been requested, calculated and is now ready to render.
        /// DO NOT use the = operator to set this. Always use the += operator to add additional subscribers to the list.
        /// </summary>
        public event Action<CelestialDataPackage<Star>> UserPositionUpdated;

        public async override void _Ready()
        {
            repositoryService = await InjectionService<Star>.GetRepositoryServiceAsync(ProjectSettings.GlobalizePath("res://"));

            var skyView = GetNode<SkyView>(viewPortPath);
            UserPositionUpdated = skyView.UpdateUserPosition;

            var constellationButton = GetNode<ConstellationButton>("ConstellationButton");
            constellationButton.ConstellationLinesToggled = skyView.ToggleConstellationLines;

            var labelButton = GetNode<LabelButton>("LabelButton");
            labelButton.ConstellationLabelsToggled = skyView.ToggleConstellationLabels;

            var azimuthButton = GetNode<AzimuthButton>("AzimuthButton");
            azimuthButton.GridlinesToggled = skyView.ToggleGridlines;

            var messierButton = GetNode<MessierButton>("MessierButton");
            messierButton.NotifyControllerOfUserUpdate = UpdateUserPosition;
        }

        /// <summary>
        /// Passed to the <see cref="Delegate"/> of the user interface to notify the controller of a new user request to be processed. 
        /// Processes the request asynchronously and passes the result to all subscribers. 
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="dateTime"></param>
        /// <returns>A task that can be awaited until all subscribers have been notified of the request.</returns>
        public async Task UpdateUserPosition(double latitude, double longitude, DateTime dateTime)
        {
            GD.Print("Updating user position");
            var dataPackage = await repositoryService.UpdateUserPosition(latitude, longitude, dateTime);
            UserPositionUpdated?.Invoke(dataPackage);
        }


    }
}