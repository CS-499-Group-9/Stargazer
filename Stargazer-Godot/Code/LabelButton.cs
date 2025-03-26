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

        /// <summary>
        /// Receives the <see cref="Signal"/> from the <see cref="LabelButton"/>
        /// </summary>
        /// <param name="state">True if the constellation labels should be shown.</param>
        private void ToggleLabel(bool state)
        {
            ConstellationLabelsToggled(state);
        }
    }
}