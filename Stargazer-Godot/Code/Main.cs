using Godot;

namespace Stargazer
{
    /// <summary>
    /// Contains the main camera used to create the user view.
    /// </summary>
    public partial class Main : Camera3D
    {
        /// <summary>
        /// The base sensitivity of the mouse used when panning the view.
        /// </summary>
        [Export] public float MouseSensitivity = 0.002f;

        private float yaw = 0f;  // Left/Right Rotation
        private float pitch = 0f; // Up/Down Rotation
        private bool rightClickHeld = false;
        private string screenshotPath = "user://screenshot.jpeg";
  
        private Globals globalVars;

        public override void _Ready()
        {
            globalVars = GetNode<Globals>("/root/Globals"); // Import globals
        }
        
        /// <summary>
        /// Checks if the right mouse button is being held down to pan the view.
        /// </summary>
        /// <param name="event"></param>
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButton)
            {
                if (mouseButton.ButtonIndex == MouseButton.Right)
                {
                    rightClickHeld = mouseButton.Pressed;
                    Input.MouseMode = rightClickHeld ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
                }

                if (mouseButton.ButtonIndex == MouseButton.WheelUp || mouseButton.ButtonIndex == MouseButton.WheelDown)
                {
                    // Check the direction of the scroll wheel
                    if (mouseButton.ButtonIndex == MouseButton.WheelUp)
                    {
                        ZoomIn();
                    }
                    else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
                    {
                        ZoomOut();
                    }
                }
            }
            if (rightClickHeld && @event is InputEventMouseMotion mouseMotion)
            {
                yaw -= (Fov / 75) * mouseMotion.Relative.X * MouseSensitivity;
                pitch -= (Fov / 75) * mouseMotion.Relative.Y * MouseSensitivity;

                // Clamp pitch to prevent flipping
                pitch = Mathf.Clamp(pitch, -Mathf.Pi / 2, Mathf.Pi / 2);

                // Apply rotation
                Rotation = new Vector3(pitch, yaw, 0);
            }
            if (!rightClickHeld && @event is InputEventMouseMotion mouseMotion2)
            {
                var worldspace = GetWorld3D().DirectSpaceState;
                var start = ProjectRayOrigin(mouseMotion2.Position);
                var end = ProjectPosition(mouseMotion2.Position, 1000);
                var result = worldspace.IntersectRay(PhysicsRayQueryParameters3D.Create(start, end));
                if (result.Count > 0)
                {
                    globalVars.isHover = true;
                    Node3D collider = result["collider"].As<Node3D>();
                    Star star = (Star)collider.GetParentNode3D();
                    globalVars.hoverLabel = star.starName;
                }
                else
                {
                    globalVars.isHover = false;
                }
            }
        }

        private void ZoomIn()
        {
            GD.Print($"{Fov}");
            // Decrease field of view for zooming in (if using a perspective camera)
            Fov = Mathf.Clamp(Fov - 2, 10, 90); // Example: Adjust sensitivity (2) and clamp the FOV
        }
        
        private void ZoomOut()
        {
            // Decrease field of view for zooming in (if using a perspective camera)
            Fov = Mathf.Clamp(Fov + 2, 10, 90); // Example: Adjust sensitivity (2) and clamp the FOV
        }


        /// <summary>
        /// Used to check for the input for the screenshot key and take the screenshot.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            // Check if the 'screenshot_key' action is pressed
            if (Input.IsActionJustPressed("screenshot_key"))
            {
                TakeScreenshot();
            }
        }

        private void TakeScreenshot()
        {
            // Get the current viewport as an Image
            Viewport viewport = GetViewport();
            Image screenshotImage = viewport.GetTexture().GetImage();

            // Save the screenshot as a JPEG
            screenshotImage.SaveJpg(screenshotPath);
            GD.Print($"Screenshot saved to {screenshotPath}");
        }
    }
}