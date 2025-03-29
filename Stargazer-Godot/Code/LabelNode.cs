using Godot;

namespace Stargazer
{
    /// <summary>
    /// Contains the constellation label.
    /// </summary>
    public partial class LabelNode : Node3D
    {
        /// <summary>
        /// Used to receive the text to display on the label on instantiation.
        /// </summary>
        [Export] public string LabelText { get; set; }

        /// <summary>
        /// Gets a reference to the <see cref="Label3D"/> and passes the text to be displayed.
        /// </summary>
        public override void _Ready()
        {
            // Get a reference to the label and pass the text.
            var child = GetChild<Label3D>(0);
            child.Text = LabelText;
        }

    }
}