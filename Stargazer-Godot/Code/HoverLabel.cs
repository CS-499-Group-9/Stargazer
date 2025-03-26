using Godot;
using Stargazer;
using System;

namespace Stargazer
{
    /// <summary>
    /// Used to display a popup label containing star/planet data when the mouse hovers over it.
    /// </summary>
    public partial class HoverLabel : Label
    {
        private Globals globalVars;

        /// <summary>
        /// Initializes the communication values on startup.
        /// </summary>
        public override void _Ready()
        {
            globalVars = GetNode<Globals>("/root/Globals"); // Import globals
            Visible = globalVars.isHover;
            AddThemeFontSizeOverride("font_size", 25);
            LabelSettings.FontSize = 30;
        }

        /// <summary>
        /// Checks to see if it should be displayed each frame.
        /// Will eventually be implemented using the observer pattern.
        /// </summary>
        /// <param name="delta">Unused number of seconds since last frame.</param>
        public override void _Process(double delta)
        {
            Visible = globalVars.isHover;
            SetText(globalVars.hoverLabel);

        }

        /// <summary>
        /// Shifts the label to the side of the mouse position.
        /// </summary>
        /// <param name="event"></param>
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion mouseMotion)
            {
                var shift = new Vector2(15f, 10f);
                Position = mouseMotion.Position + shift;
            }
        }
    }
}