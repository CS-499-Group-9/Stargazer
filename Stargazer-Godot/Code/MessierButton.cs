using Godot;
using System;
using System.Threading.Tasks;

namespace Stargazer
{
    /// <summary>
    /// Contains the button used to toggle the visibility of the Messier Deep Space Objects
    /// </summary>
    public partial class MessierButton : Control
    {
        /// <summary>
        /// Used to broadcast the notification to toggle the visibility of Messier Deep Space Objects.
        /// </summary>
        public Action<bool> MessierObjectsToggled;

        /// <summary>
        /// Receives the <see cref="Signal"/> from the <see cref="CheckBox"/>
        /// </summary>
        /// <param name="state">True if the Messier Objects should be visible.</param>
        private void ToggleMessierObjects(bool state)
        {
            MessierObjectsToggled(state);
        }
    }
}