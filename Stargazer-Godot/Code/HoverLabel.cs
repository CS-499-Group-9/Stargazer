using Godot;

namespace Stargazer
{
    /// <summary>
    /// The label that is displayed when hovering over an <see cref="IHoverable"/> object.
    /// </summary>
    public partial class HoverLabel : Label
    {
        private Globals globalVars;
        private Vector2 shift = new (15f, 10f);
        IHoverable hoveredBody;
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
            Position = GetViewport().GetMousePosition() + shift;
            SetText(hoveredBody?.GetHoverText() ?? "");
            Visible = hoveredBody is not null;

        }

        public void HoverableChangeHandler(IHoverable hoverable)
        {
            hoveredBody = hoverable;
        }
    }
}