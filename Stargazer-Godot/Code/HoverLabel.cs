using Godot;

namespace Stargazer
{
    /// <summary>
    /// The label that is displayed when hovering over an <see cref="IHoverable"/> object.
    /// </summary>
    public partial class HoverLabel : Label
    {
        private Vector2 shift = new (15f, 10f);
        private IHoverable hoveredBody;
        /// <summary>
        /// Initializes label settings
        /// </summary>
        public override void _Ready()
        {
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
        /// <summary>
        /// Receives notification of the mouse hovering over a new object.
        /// </summary>
        /// <param name="hoverable">The object the mouse is hovering over.</param>
        public void HoverableChangeHandler(IHoverable hoverable)
        {
            hoveredBody = hoverable;
        }
    }
}