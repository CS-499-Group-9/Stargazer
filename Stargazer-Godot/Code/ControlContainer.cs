using DataLayer;
using Godot;
using Stargazer;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Stargazer
{
    /// <summary>
    /// Contains all user controls for the program and notifies subscribers of user interaction. 
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    public partial class ControlContainer : Control
    {
        [Export] private LineEdit latDegField;
        [Export] private LineEdit latMinField;
        [Export] private LineEdit lonDegField;
        [Export] private LineEdit lonMinField;
        [Export] private LineEdit timeField;
        [Export] private OptionButton AMorPMButton;
        [Export] private Button calendarButton;
        [Export] private OptionButton formatSelector;
        [Export] private HSlider TimeLapseSlider;
        [Export] private Label TimeLapseLabel;


        private Startup _mainControl;
        private DateTime baseDateTime; // Start time of timelapse.
        private TimeSpan totalSpan = TimeSpan.FromHours(24); // Have it timelapse a day.
        private int frameCount = 100; // Number of frames to use.
        private double _lastLatitude; // Last latitude position for the timelapse.
        private double _lastLongitude; // Last longitude position for the timelapse.
        private bool _exportGifReversed = false; // Reverse gif if true.

        /// <summary>
        /// Notifies the subscribers when the user has toggled the azimuth grid
        /// </summary>
        public Action<bool> AzimuthToggled;
        /// <summary>
        /// Notifies the subscribers when the user has toggled the equatorial grid
        /// </summary>
        public Action<bool> EquatorialToggled;
        /// <summary>
        /// Notifies the subscribers when the user has toggle the visibility of constellations
        /// </summary>
        public Action<bool> ConstellationsToggled;
        /// <summary>
        /// Notifies the subscribers when the user has toggled the visibility of constellation labels
        /// </summary>
        public Action<bool> ConstellationLabelsToggled;
        /// <summary>
        /// Notifies the subscribers when the user has toggled the visibility of Messier objects
        /// </summary>
        public Action<bool> MessierObjectsToggled;
        /// <summary>
        /// Notifies the subscriber (one only) of a request to update to the observers position/time.
        /// </summary>
        public Action<double, double, DateTime> UserPositionUpdated;
        /// <summary>
        /// Broadcasts a request to take a screenshot
        /// </summary>
        public Func<Task> RequestScreenshot;

        /// <summary>
        /// Checks to see if the user has requested a screenshot
        /// </summary>
        /// <param name="delta">Unused</param>
        public async override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("screenshot_key"))
            {
                await RequestScreenshot();
            }
        }

        /// <summary>
        /// Receives the <see cref="Signal"/> from the AzimuthButton's <see cref="CheckBox"/> and broadcasts on the <see cref="AzimuthToggled"/> notification.
        /// </summary>
        /// <param name="value">True if the toggle button is checked</param>
        public void ToggleAzimuth(bool value) { AzimuthToggled?.Invoke(value); }

        /// <summary>
        /// Receives the <see cref="Signal"/> from the <c>Toggle Equatorial</c>
        /// </summary>
        /// <param name="value">True if the toggle button is checked.</param>
        public void ToggleEquatorial(bool value) { EquatorialToggled?.Invoke(value); }

        /// <summary>
        /// Receives the <see cref="Signal"/> from the ConstellationButton's <see cref="CheckBox"/> and broadcasts on the <see cref="ConstellationsToggled"/> notification.
        /// </summary>
        /// <param name="value">True if the toggle button is checked</param>
        public void ToggleConstellations(bool value) { ConstellationsToggled?.Invoke(value); }

        /// <summary>
        /// Receives the <see cref="Signal"/> from the LabelButton's <see cref="CheckBox"/> and broadcasts on the <see cref="ConstellationLabelsToggled"/> notification.
        /// </summary>
        /// <param name="value">True if the checkbox is checked</param>
        public void ToggleConstellationLabels(bool value) { ConstellationLabelsToggled?.Invoke(value); }


        /// <summary>
        /// Receives the <see cref="Signal"/> from the "MessierButton's <see cref="CheckBox"/> and broadcasts on the <see cref="MessierObjectsToggled"/> notification.
        /// </summary>
        /// <param name="value">True if the checkbox is checked</param>
        public void ToggleMessierObjects(bool value) { MessierObjectsToggled?.Invoke(value); }

        /// <summary>
        /// Sets a reference to the main controller in the control container.
        /// </summary>
        /// <param name="controller">The main controller</param>
        public void SetMainController(Startup controller)
        {
            _mainControl = controller;
        }

        private void OnSliderGuiInput(InputEvent @event)
        {
            if (TimeLapseLabel == null)
                return;

            if (@event is InputEventMouseButton mouseEvent)
            {
                if (!mouseEvent.Pressed)
                {
                    // Reset to the default label when you're done using the slider
                    TimeLapseLabel.Text = "Timelapse Slider";
                }
            }
        }

        /// <summary>
        /// Receives the <c>Enter</c> button's <see cref="Signal"/>, gathers user input and broadcasts <see cref="UserPositionUpdated"/>
        /// Author: Logan Parker
        /// Created: SPR 2025
        /// Modified: Alex Lake SPR 2025 (added time lapse slider)
        /// </summary>
	    public void UpdateUserPosition()
        {
            // Get the raw values from the text boxes
            string latDegText = latDegField.Text.Trim();
            string latMinText = latMinField.Text.Trim();
            string lonDegText = lonDegField.Text.Trim();
            string lonMinText = lonMinField.Text.Trim();
            string timeText = timeField.Text.Trim();
            string dateText = calendarButton.Text.Trim();
            string amPmText = AMorPMButton.GetItemText(AMorPMButton.Selected);


            // Check if any field is empty
            if (string.IsNullOrEmpty(latDegText) || string.IsNullOrEmpty(latMinText) ||
                string.IsNullOrEmpty(lonDegText) || string.IsNullOrEmpty(lonMinText) ||
                string.IsNullOrEmpty(timeText) || string.IsNullOrEmpty(dateText))
            {
                // TODO: This should display something useful to the user.
                GD.PrintErr("Please fill out all fields before submitting.");
                return;
            }

            // Try parsing values
            if (!double.TryParse(latDegText, out double latDeg) ||
                !double.TryParse(latMinText, out double latMin) ||
                !double.TryParse(lonDegText, out double lonDeg) ||
                !double.TryParse(lonMinText, out double lonMin))
            {
                // TODO: This should display something useful to the user.
                GD.PrintErr("Invalid number entered in latitude/longitude.");
                return;
            }

            // Convert the minutes to decimal degrees and add to the whole values.
            double latitude = latDeg + latMin * 0.016667;
            double longitude = lonDeg + lonMin * 0.016667;

            // Parse time
            string[] timeSplit = timeText.Split(':');
            if (timeSplit.Length != 2 ||
                !int.TryParse(timeSplit[0], out int hour) ||
                !int.TryParse(timeSplit[1], out int minute))
            {
                // TODO: This should display something useful to the user. 
                GD.PrintErr("Invalid time format.");
                return;
            }

            if (amPmText == "PM" && hour < 12) hour += 12;
            if (amPmText == "AM" && hour == 12) hour = 0;

            if (!DateTime.TryParse($"{dateText} {hour:00}:{minute:00}:00", out DateTime localDateTime))
            {
                // TODO: This should display something useful to the user.
                GD.PrintErr("Invalid date format.");
                return;
            }

            // Treat the user input as local time, then convert to UTC
            DateTime utcDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local).ToUniversalTime();

            // All checks passed
            SetBaseDateTime(utcDateTime);
            TimeLapseSlider.Value = 0;
            UserPositionUpdated(latitude, longitude, utcDateTime);

            _lastLatitude = latitude;
            _lastLongitude = longitude;
        }

        /// <summary>
        /// Toggles a flag when the user selects/deselects the reverse gif checkbox.
        /// Author: Alex Lake
        /// Created: SPR 2025
        /// </summary>
        /// <param name="toggled"></param>
        public void _on_reverse_gif_toggled(bool toggled)
        {
            _exportGifReversed = toggled;
        }

        /// <summary>
        /// Sets the base date time for the time lapse.
        /// Author Alex Lake
        /// Created: SPR 2025
        /// </summary>
        /// <param name="dateTime">The time to set.</param>
        public void SetBaseDateTime(DateTime dateTime)
        {
            baseDateTime = dateTime; // Set the base date and time.
        }

        /// <summary>
        /// Called each time the time lapse advances one frame.
        /// Author: Alex Lake
        /// Created: SPR 2025
        /// </summary>
        /// <param name="frame">The amount of time passed since the last frame</param>
        private void OnTimeLapseFrameChanged(double frame)
        {
            double progress = frame / (frameCount - 1);
            DateTime targetTime = baseDateTime + TimeSpan.FromTicks((long)(totalSpan.Ticks * progress));
            GD.Print($"Time-lapse frame: {frame}, datetime: {targetTime}");

            // Label Update
            if (TimeLapseLabel != null)
            {
                var localTime = targetTime.ToLocalTime();
                TimeLapseLabel.Text = $"Selected Time: {localTime:hh:mm:ss tt}";
            }

            // Use the existing broadcast pattern
            UserPositionUpdated?.Invoke(_lastLatitude, _lastLongitude, targetTime.ToUniversalTime());
        }

        /// <summary>
        /// Screenshot button
        /// Author: Alex Lake
        /// Created: SPR 2025
        /// </summary>
        private async void _on_button_pressed()
        {
            if (_mainControl != null)
            {
                await _mainControl.TakeScreenshot();
            }
            else
            {
                // TODO: Refactor using C# Action
                GD.PrintErr("Main controller reference not set!");
            }
        }

        /// <summary>
        /// Notifies the main controller of a user request to export a time lapse
        /// Author: Alex Lake
        /// Created: SPR 2025
        /// </summary>
        private async void _on_timelapse_gif_export_button_pressed()
        {
            if (_mainControl != null)
            {
                await _mainControl.ExportTimelapseGif(_lastLatitude, _lastLongitude, baseDateTime, _exportGifReversed);
            }
            else
            {
                // TODO: Refactor using C# Action
                GD.PrintErr("Main control is not assigned.");
            }
        }

        /// <summary>
        /// Returns the screenshot format that is currently selected
        /// Author: Alex Lake
        /// Created: SPR 2025
        /// </summary>
        /// <returns>The currently selected format</returns>
        public string GetSelectedScreenshotFormat()
        {
            return formatSelector.GetItemText(formatSelector.Selected);
        }
    }
}
