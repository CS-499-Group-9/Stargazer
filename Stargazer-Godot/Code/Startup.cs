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
        private PlayControl playControl;
        private SkyView skyView;

        [Export] private Control control;
        [Export] private SkyViewContainer skyViewContainer;

        /// <summary>
        /// A <see cref="Delegate"/> used to notify the viewport that new star data has been requested, calculated and is now ready to render.
        /// </summary>
        public event Func<CelestialDataPackage<Star>, Task> UserPositionUpdated;


        /// <summary>
        /// Creates the repository service and stores in memory
        /// Gathers references to sender/receiver nodes and connects <see cref="Delegate"/>s for communication.
        /// </summary>
        public async override void _Ready()
        {

            repositoryService = await InjectionService<Star>.GetRepositoryServiceAsync(ProjectSettings.GlobalizePath("res://"));

            var controlContainer = GetNode<ControlContainer>(nameof(ControlContainer));
            var skyViewContainer = GetNode<SkyViewContainer>(nameof(SkyViewContainer));
            skyView = skyViewContainer.SkyView;
            playControl = GetNode<PlayControl>(nameof(PlayControl));

            controlContainer.AzimuthToggled = skyView.ToggleGridlines;
            controlContainer.EquatorLinesToggled = skyView.ToggleEquatorialGridlines;
            controlContainer.ConstellationsToggled = skyView.ToggleConstellationLines;
            controlContainer.ConstellationLabelsToggled = skyView.ToggleConstellationLabels;
            controlContainer.UserPositionUpdated = UpdateUserPosition;
           


            UserPositionUpdated = skyView.UpdateUserPosition;

        }

        /// <summary>
        /// Passed to the <see cref="Delegate"/> of the user interface to notify the controller of a new user request to be processed. 
        /// Processes the request asynchronously and passes the result to the viewport
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="dateTime"></param>
        /// <returns>A task that can be awaited until all subscribers have been notified of the request.</returns>
        public async Task UpdateUserPosition(double latitude, double longitude, DateTime dateTime)
        {
            // Uncomment the timers to make it advance.

            var multiplier = playControl.Activate();
            skyView.SetTimeMultiplier(multiplier);
            var dataPackage = repositoryService.UpdateUserPosition(latitude, longitude, dateTime);
            await UserPositionUpdated(dataPackage);

        }


    }
}