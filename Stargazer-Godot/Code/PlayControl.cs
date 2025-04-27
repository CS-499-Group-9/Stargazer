using Godot;
using Stargazer;
using System;

namespace Stargazer
{
    /// <summary>
    /// Controls the play speed of the 3D simulation.
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    public partial class PlayControl : Control
    {
        private Label multiplierLabel;
        private PlaySpeed multiplier;

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
            multiplierLabel.Text = $"{multiplier.ToString()} per second";
        }

    }
}