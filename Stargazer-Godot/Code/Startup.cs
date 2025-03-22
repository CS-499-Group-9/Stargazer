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
        /// A <see cref="Delegate"/> used to notify the viewport that new star data has been requested, calculated and is now ready to render.
        /// </summary>
        public event Func<CelestialDataPackage<Star>, Task> UserPositionUpdated;
       

        private Timer timer;
        private double latitude, longitude;
        private DateTime userTime;

        /// <summary>
        /// Creates the repository service and stores in memory
        /// Gathers references to sender/receiver nodes and connects <see cref="Delegate"/>s for communication.
        /// </summary>
        public async override void _Ready()
        {
            repositoryService = await InjectionService<Star>.GetRepositoryServiceAsync(ProjectSettings.GlobalizePath("res://"));

            var controlContainer = GetNode<ControlContainer>(nameof(ControlContainer));
            var skyViewContainer = GetNode<SkyViewContainer>(nameof(SkyViewContainer));
            var skyView = skyViewContainer.SkyView;

            controlContainer.AzimuthToggled = skyView.ToggleGridlines;
            controlContainer.ConstellationsToggled = skyView.ToggleConstellationLines;
            controlContainer.ConstellationLabelsToggled = skyView.ToggleConstellationLabels;
            controlContainer.UserPositionUpdated = UpdateUserPosition;


            //var gridText = GetNode<GridLabel>(viewPortPath + "/GridLabel");

            UserPositionUpdated = skyView.UpdateUserPosition;

            
            timer = new Timer
            {
                WaitTime = 1.5f,
                Autostart = false,
                OneShot = false
            };
            timer.Timeout += OnTimer;
            AddChild(timer);
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
            //timer.Stop();
            this.latitude = latitude;
            this.longitude = longitude;
            this.userTime  = dateTime;
            var dataPackage = await repositoryService.UpdateUserPosition(latitude, longitude, dateTime);
            UserPositionUpdated?.Invoke(dataPackage);
            //timer.Start();
        }

        private async void OnTimer()
        {
            // Change the value in AddMinutes to change how much the sky changes per iteration
            userTime = userTime.AddMinutes(10);
            GD.Print($"{userTime}");
            var dataPackage = await repositoryService.UpdateUserPosition(latitude, longitude, userTime);
            UserPositionUpdated?.Invoke(dataPackage);
        }
    }
}