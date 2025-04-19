using DataLayer;
using DataLayer.Interfaces;
using Godot;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Tiff;
using ImageSharpImage = SixLabors.ImageSharp.Image;

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
                DirectoryInfo dir = new DirectoryInfo(path) ?? throw new DirectoryNotFoundException($"{path} is not a valid directory");
                path = Path.Combine(dir.Parent.FullName, "DataLayer") ?? throw new DirectoryNotFoundException();
            }
            else
            {
                path = OS.GetExecutablePath().GetBaseDir();

            }
            var repositoryService =  InjectionService<Star>.GetRepositoryServiceAsync(path);
            dataPackage = await repositoryService.InitializeDataPackage();
            calculator = dataPackage.Calculator;

            calculator.SetTime(DateTime.UtcNow);

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

        private void SaveAsGif(Godot.Image godotImage, int width, int height, string gifPath) // This function takes an image and creates a single frame gif.
        {
            godotImage.Convert(Godot.Image.Format.Rgba8); // Converts the Godot image to an RGBA8 format.
            byte[] rawData = godotImage.GetData(); // Get the raw data of the image.

            using (var image = ImageSharpImage.LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(rawData, width, height))
            {
                image.Metadata.GetGifMetadata().RepeatCount = 0; // Set the loop behavior (0 = loop forever).

                var encoder = new SixLabors.ImageSharp.Formats.Gif.GifEncoder(); // Encode the image as a gif.
                image.Save(gifPath, encoder); // Save the image to the path using the gif encoder.
            }
        }

        private void SaveWithImageSharp(Godot.Image godotImage, int width, int height, string path, IImageEncoder encoder) // This function helps encode the images properly using ImageSharp.
        {
            godotImage.Convert(Godot.Image.Format.Rgba8); // Convert the Godot Image to format RGBA8.
            byte[] imageBytes = godotImage.GetData(); // Get the data of the image.

            using var image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(imageBytes, width, height); // Load the data of the image.
            image.Save(path, encoder); // Save the image using the correct encoder.
        }

        public async Task TakeScreenshot()
        {
            var skyView2d = View2D.GetNode<SkyView2D>("View2d");
            await skyView2d.UpdateUserPosition(dataPackage, calculator.getTime(), calculator.getLongLat());
            string selectedFormat = controlContainer.GetSelectedScreenshotFormat().ToLower();

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

            string format = controlContainer.GetSelectedScreenshotFormat().ToLower();
            string extension = format == "jpeg" ? "jpg" : format;

            // Create the filename using latitude, longitude, and the date
            screenshotPath = Path.Combine(screenshotDir, $"Screenshot_{latStr}_{lonStr}_{formattedDate}.{extension}");


            // Wait for 1 second before taking the screenshot
            Timer screenshotTimer = new Timer();
            screenshotTimer.WaitTime = 1;
            screenshotTimer.OneShot = true;
            screenshotTimer.Timeout += () => { ExportScreenshot(View2D, selectedFormat); };
            screenshotTimer.Autostart = true;
            AddChild(screenshotTimer);
        }

       public async Task ExportTimelapseGif(double latitude, double longitude, DateTime startTime)
{
    const int frameCount = 60;
    const int frameIntervalMinutes = 1;
    int width = 2550;
    int height = 3300;

    string outputDir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "Stargazer GIF Frames");
    Directory.CreateDirectory(outputDir);
    List<Image<Rgba32>> gifFrames = new();

    // Assuming the ProgressBar is already created in the Godot editor and assigned via export
    var progressBar = GetNode<ProgressBar>("ProgressBar");

    // Set the maximum value of the progress bar to the frame count
    progressBar.MaxValue = frameCount;

    for (int i = 0; i < frameCount; i++)
    {
        DateTime currentTime = startTime.AddMinutes(i);
        calculator.SetLocation(latitude, longitude);
        calculator.SetTime(currentTime);

        var skyView2d = View2D.GetNode<SkyView2D>("View2d");
        await skyView2d.UpdateUserPosition(dataPackage, currentTime, calculator.getLongLat());

        // Wait for the frame to render
        await ToSignal(GetTree(), "process_frame");  // Yield control to the main loop for frame processing

        Godot.Image godotImage = View2D.GetTexture().GetImage();
        godotImage.Resize(width, height);
        godotImage.Convert(Godot.Image.Format.Rgba8);
        byte[] raw = godotImage.GetData();

        var img = ImageSharpImage.LoadPixelData<Rgba32>(raw, width, height);
        img.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = 5; // ~0.5s per frame
        gifFrames.Add(img.Clone());

        // Update the progress bar
        progressBar.Value = i + 1; // Update the progress bar based on the current frame

        // Yield control to allow UI updates
        await ToSignal(GetTree(), "process_frame");  // This will make sure the UI updates after each frame.
    }

    string gifPath = Path.Combine(outputDir, $"Timelapse_{startTime:yyyyMMdd_HHmmss}.gif");
    using (var gif = new Image<Rgba32>(gifFrames[0].Width, gifFrames[0].Height))
    {
        for (int i = 0; i < gifFrames.Count; i++)
        {
            gif.Frames.AddFrame(gifFrames[i].Frames.RootFrame);
        }

        gif.Frames.RemoveFrame(0); // Remove placeholder root frame
        gif.Metadata.GetGifMetadata().RepeatCount = 0;
        gif.Save(gifPath, new GifEncoder());
    }

    GD.Print($"GIF saved to: {gifPath}");

    // Optionally reset the progress bar value after the process
    progressBar.Value = 0;
}




        private void ExportScreenshot(SubViewport view2D, string format)
        {
            // Define the required resolution (300 DPI for 8.5x11 inches)
            int width = 2550;  // 8.5 inches * 300 DPI
            int height = 3300; // 11 inches * 300 DPI

            // Get the image from the viewport
            Godot.Image screenshotImage = view2D.GetTexture().GetImage();
    
            // Resize the image to fit the 8.5x11 dimensions at 300 DPI
            screenshotImage.Resize(width, height);

            switch (format)
            {
                case "png":
                    screenshotImage.SavePng(screenshotPath);
                    break;
                case "jpeg":
                    screenshotImage.SaveJpg(screenshotPath, 90);
                    break;
                case "bmp":
                    SaveWithImageSharp(screenshotImage, width, height, screenshotPath, new BmpEncoder());
                    break;
                case "webp":
                    screenshotImage.SaveWebp(screenshotPath);
                    break;
                case "gif":
                    SaveAsGif(screenshotImage, width, height, screenshotPath);
                    break;
                case "tiff":
                    SaveWithImageSharp(screenshotImage, width, height, screenshotPath, new TiffEncoder());
                    break;
            }
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