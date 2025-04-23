using Godot;
using Stargazer;
using System;

namespace Stargazer
{
    
    /// <summary>
    /// Controls the play speed of the 3D simulation.
    /// </summary>
    public partial class PlayControl : Control
    {
        private Label multiplierLabel;
        private PlaySpeed multiplier;

        private DateTime baseDateTime; // Start time of timelapse.

        private int frameCount = 100; // Number of frames to use.
        private double _lastLatitude; // Last latitude position for the timelapse.
        private double _lastLongitude; // Last longitude position for the timelapse.

	    public Action<double, double, DateTime> UserPositionUpdated;

        private TimeSpan totalSpan = TimeSpan.FromHours(24); // Have it timelapse a day.

        [Export] private HSlider TimeLapseSlider;
        [Export] private Label TimeLapseLabel;

        /// <summary>
        /// Receives the <see cref="Signal"/> from the <c>Sec</c> buttons
        /// </summary>
        /// <param name="seconds">1 or -1</param>
        public void UpdateSeconds(int seconds)
        {
            multiplier.IncreaseBySeconds(seconds);
            UpdateMultiplierLabel();
        }
        /// <summary>
        /// Receives the <see cref="Signal"/> form the <c>Min</c> buttons
        /// </summary>
        /// <param name="minutes">1 or -1</param>
        public void UpdateMinutes(int minutes)
        {
            multiplier.IncreaseByMinutes(minutes);
            UpdateMultiplierLabel();
        }
        /// <summary>
        /// Receives the <see cref="Signal"/> from the <c>Hrs</c> buttons
        /// </summary>
        /// <param name="hours"></param>
        public void UpdateHours(int hours)
        {
            multiplier.IncreateByHours(hours);
            UpdateMultiplierLabel();
        }

        /// <summary>
        /// Receives the <see cref="Signal"/> from the <c>Days</c> buttons
        /// </summary>
        /// <param name="days"></param>
        public void UpdateDays(int days)
        {
            multiplier.IncreaseByDays(days);
            UpdateMultiplierLabel();
        }

        /// <summary>
        /// Syncronized to current time.
        /// </summary>
        public void OnSynchronizeTime()
        {
            multiplier.SynchronizeTime();
            UpdateMultiplierLabel();
        }

        /// <summary>
        /// Sets the play speed to 1:1
        /// </summary>
        public void PlayNormal()
        {
            multiplier.RealTime();
            UpdateMultiplierLabel();
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public override void _Ready()
        {
            multiplier = new PlaySpeed();
            multiplierLabel = GetNode<Label>("MarginContainer/VBoxContainer/MultiplierLabel");
            UpdateMultiplierLabel();

            TimeLapseSlider.MinValue = 0;
            TimeLapseSlider.MaxValue = frameCount - 1;
            TimeLapseSlider.Step = 1;
            TimeLapseSlider.ValueChanged += OnTimeLapseFrameChanged;

            // Hide the label
            if (TimeLapseLabel != null)
            {
                TimeLapseLabel.Text = "Timelapse Slider"; // Default text
                TimeLapseLabel.Visible = true;
            }

            TimeLapseSlider.GuiInput += OnSliderGuiInput;

            // Set the initial timelapse slider time to clock time
            SetBaseDateTime(DateTime.UtcNow);
            _lastLatitude = 0;
            _lastLongitude = 0;

        }

        /// <summary>
        /// Used to activate the object.
        /// </summary>
        /// <returns>The <see cref="PlaySpeed"/> state object.</returns>
        public PlaySpeed Activate()
        {
            Visible = true;
            UpdateMultiplierLabel();
            return multiplier;
        }

        private void UpdateMultiplierLabel()
        {
        multiplierLabel.Text = $"x{multiplier.TotalSeconds:0} Speed ({multiplier.ToString()} per second)";

        }
        public void SetBaseDateTime(DateTime dateTime)
        {
            baseDateTime = dateTime; // Set the base date and time.
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


    }
}