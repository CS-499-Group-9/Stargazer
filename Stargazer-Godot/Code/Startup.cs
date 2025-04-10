using DataLayer;
using DataLayer.Interfaces;
using Godot;
using System;
using System.Diagnostics;
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
        private SkyView skyView;
        private CelestialDataPackage<Star> dataPackage;
        private IEquatorialCalculator calculator;
        private string screenshotPath;

        [Export] private SubViewport View2D;
        [Export] private ControlContainer controlContainer;
        [Export] private SkyViewContainer skyViewContainer;
        [Export] private PlayControl playControl;
        [Export] private AcceptDialog ScreenshotDialog;
        [Export] private ColorRect ModalBlocker;


        /// <summary>
        /// Creates the repository service and stores in memory
        /// Gathers references to sender/receiver nodes and connects <see cref="Delegate"/>s for communication.
        /// </summary>
        public async override void _Ready()
        {
            var path = "";
            if (OS.HasFeature("editor"))
            {
                path = ProjectSettings.GlobalizePath("res://");
            }
            else
            {
                path = OS.GetExecutablePath().GetBaseDir();

            }
            var repositoryService =  InjectionService<Star>.GetRepositoryServiceAsync(path);
            dataPackage = await repositoryService.InitializeDataPackage();
            calculator = dataPackage.Calculator;
            skyView = skyViewContainer.SkyView;
            await skyView.InitializeCelestial(dataPackage);

            // Set up subscribers to notifications.
            controlContainer.AzimuthToggled = skyView.ToggleGridlines;
            controlContainer.EquatorialToggled = skyView.ToggleEquatorialGridlines;
            controlContainer.MessierObjectsToggled = skyView.ToggleMessierObjects;
            controlContainer.ConstellationsToggled = skyView.ToggleConstellationLines;
            controlContainer.ConstellationLabelsToggled = skyView.ToggleConstellationLabels;
            controlContainer.UserPositionUpdated = UpdateUserPosition;
            controlContainer.RequestScreenshot = TakeScreenshot;

            // Activate the Play Controller and notify the SkyView
            var multiplier = playControl.Activate();
            skyView.SetTimeMultiplier(multiplier);
            controlContainer.SetMainController(this);

            string picturesPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
            string screenshotDir = System.IO.Path.Combine(picturesPath, "Stargazer Screenshots");

             // Ensure the folder exists
             System.IO.Directory.CreateDirectory(screenshotDir);

             // Create timestamped filename
             string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
             screenshotPath = System.IO.Path.Combine(screenshotDir, $"Screenshot_{timestamp}.png");
        }

        /// <summary>
        /// Passed to the <see cref="Delegate"/> of the user interface to notify the controller of a new user request to be processed. 
        /// Processes the request asynchronously and passes the result to the viewport
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="dateTime"></param>
        /// <returns>A task that can be awaited until all subscribers have been notified of the request.</returns>
        public void UpdateUserPosition(double latitude, double longitude, DateTime dateTime)
        {
            // Uncomment the timers to make it advance.

            calculator.SetLocation(latitude, longitude);
            calculator.SetTime(dateTime);

        }

        public async Task TakeScreenshot()
        {
            var skyView2d = View2D.GetNode<SkyView2D>("View2d");
            await skyView2d.UpdateUserPosition(dataPackage, calculator.getTime(), calculator.getLongLat());
    
            // Get longitude, latitude, and date from the calculator
            double latitude = calculator.Latitude;
            double longitude = calculator.Longitude;
            DateTime skyDate = calculator.getTime();  // Get the current time

            // Get formatted latitude and longitude strings
            var (latStr, lonStr) = calculator.getLongLat();
    
            // Format the date as a string (to use in the filename)
            string formattedDate = skyDate.ToString("yyyy-MM-dd_HH-mm-ss");

            // Construct the screenshot directory and ensure it exists
            string screenshotDir = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "Stargazer Screenshots");
            System.IO.Directory.CreateDirectory(screenshotDir);

            // Create the filename using latitude, longitude, and the date
            screenshotPath = System.IO.Path.Combine(screenshotDir, $"Screenshot_{latStr}_{lonStr}_{formattedDate}.jpg");

            // Wait for 1 second before taking the screenshot
            Timer screenshotTimer = new Timer();
            screenshotTimer.WaitTime = 1;
            screenshotTimer.OneShot = true;
            screenshotTimer.Timeout += () => { ExportScreenshot(View2D); };
            screenshotTimer.Autostart = true;
            AddChild(screenshotTimer);
        }


        private void ExportScreenshot(SubViewport view2D)
        {
            // Define the required resolution (300 DPI for 8.5x11 inches)
            int width = 2550;  // 8.5 inches * 300 DPI
            int height = 3300; // 11 inches * 300 DPI

            // Get the image from the viewport
            Image screenshotImage = view2D.GetTexture().GetImage();
    
            // Resize the image to fit the 8.5x11 dimensions at 300 DPI
            screenshotImage.Resize(width, height);

            screenshotImage.SaveJpg(screenshotPath, 90);

            GD.Print($"Screenshot saved to {screenshotPath}");
            ShowScreenshotSavedNotification();
        }

        private void ShowScreenshotSavedNotification()
        {
            ScreenshotDialog.DialogText = $"Screenshot saved at:\n{screenshotPath}";

            // Show the blocker
            ModalBlocker.Visible = true;

            // Connect the OK button press event (no need to check manually if connected)

            // Show the dialog
            ScreenshotDialog.PopupCentered();
        }

        private void _on_screenshot_dialog_close_requested()
        {
            OnScreenshotDialogClosed();
        }

        private void _on_screenshot_dialog_confirmed()
        {
            OnScreenshotDialogClosed();
        }

        private void OnScreenshotDialogClosed()
        {
            GD.Print("User acknowledged screenshot notification.");
            ModalBlocker.Visible = false; // Allow UI interaction again
        }
    }
}