using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// Contains the button used to toggle the azimuth lines
    /// </summary>
    public partial class AzimuthButton : Control
    {
        /// <summary>
        /// The <see cref="Delegate"/> used to notify the viewport to toggle the lines.
        /// </summary>
        public Action<bool> GridlinesToggled;

        private void ToggleAzimuth(bool state)
        {
            GridlinesToggled(state);
        }
    }
}