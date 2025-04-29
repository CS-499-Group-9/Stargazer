using Godot;

namespace Stargazer
{
    /// <summary>
    /// Displays a compass needle to orient the user relative to North.
    /// Author: Logan Parker
    /// Refactored By: Josh Johner (moved needle.RotationDegrees from _Process() to RotationHandler
    /// Created: SPR 2025
    /// </summary>
    public partial class CompassNeedle : Sprite2D
    {
        /// <summary>
        /// The needle to display
        /// </summary>
        [Export] public Node2D needle;

        /// <summary>
        /// Gets a reference to the <see cref="Camera3D"/> in the <see cref="SkyView"/>
        /// </summary>
        public override void _Ready()
        {
            Node viewNode = GetTree()?.Root.FindChild("View", true, false);

        }

        /// <summary>
        /// Receives notifications when the camera rotation has changed to rotate the needle.
        /// </summary>
        /// <param name="camera">A reference to the main camera</param>
        public void RotationHandler(Camera3D camera)
        {
            float yRotationDegrees = Mathf.RadToDeg(camera.Rotation.Y);
            needle.RotationDegrees = yRotationDegrees + 90; // Camera starts West for some reason.
        }


    }
}