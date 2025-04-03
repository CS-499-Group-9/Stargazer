using Godot;

namespace Stargazer
{
    /// <summary>
    /// The label that is displayed when hovering over an <see cref="IHoverable"/> object.
    /// </summary>
    public partial class HoverLabel : Label
    {
        private Globals globalVars;
        private Vector2 mousePos;

        /// <summary>
        /// Initializes label settings
        /// </summary>
        public override void _Ready()
        {
            globalVars = GetNode<Globals>("/root/Globals"); // Import globals
            Visible = globalVars.isHover;
            AddThemeFontSizeOverride("font_size", 25);
            LabelSettings.FontSize = 30;
        }

        /// <summary>
        /// Determines how and when to display a label for each frame.
        /// </summary>
        /// <param name="delta">Unused</param>
        public override void _Process(double delta)
        {
            Visible = globalVars.isHover;
            SetText(globalVars.hoverLabel);
            Position = mousePos;
        }

        /// <summary>
        /// Detects mouse motion and places the label next to the cursor.
        /// </summary>
        /// <param name="event"></param>
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion mouseMotion)
            {
                var shift = new Vector2(15f, 10f);
                mousePos = mouseMotion.Position + shift;
            }
        }
    }
}