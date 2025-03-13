using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// Contains the button used to toggle the constellation labels.
    /// </summary>
    public partial class LabelButton : Control
    {
        /// <summary>
        /// Contains the <see cref="Delegate"/> used to notify the viewport to toggle the constellation label visibility. 
        /// </summary>
        public Action<bool> ConstellationLabelsToggled;

        private void ToggleLabel(bool state)
        {
            ConstellationLabelsToggled(state);
        }
    }
}