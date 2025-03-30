using System;
using Godot;

namespace Stargazer
{
    /// <summary>
    /// Contains the main camera used to create the user view.
    /// </summary>
    public partial class MainCamera : Camera3D
    {
        /// <summary>
        /// The base sensitivity of the mouse used when panning the view.
        /// </summary>
        [Export] public float MouseSensitivity = 0.002f;
    
        [Export] public Vector2 ScreenOffset = new Vector2(500, 50);  // Desired screen space position

        private float yaw = 0f;  // Left/Right Rotation
        private float pitch = 0f; // Up/Down Rotation
        private bool rightClickHeld = false;
        private bool middleMouseClicked = false;
        private bool tracking = false;
        private string screenshotPath = "user://screenshot.jpeg";

        private IHoverable highlightingStar;
  
        private Globals globalVars;

        /// <inheritdoc/>
        public override void _Ready()
        {
            globalVars = GetNode<Globals>("/root/Globals"); // Import globals
        }
        
        /// <summary>
        /// Checks if the right mouse button is being held down to pan the view.
        /// </summary>
        /// <param name="event">The event data passed in.</param>
        public override void _Input(InputEvent @event)
        {
            middleMouseClicked = false;
            if(@event.IsAction("forward")){

                GD.Print("go!");
                Position -= 0.5f*Basis.Z;

            }
            if (@event is InputEventMouseButton mouseButton)
            {
                if (mouseButton.ButtonIndex == MouseButton.Right)
                {
                    globalVars.isHover = false;
                    rightClickHeld = mouseButton.Pressed;
                    Input.MouseMode = rightClickHeld ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
                }
                if (mouseButton.ButtonIndex == MouseButton.Middle){
                    middleMouseClicked = mouseButton.Pressed;
                    GD.Print("middle click");
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
            if (rightClickHeld && @event is InputEventMouseMotion mouseMotion && !tracking)
            {
                yaw -= (Fov / 75) * mouseMotion.Relative.X * MouseSensitivity;
                pitch -= (Fov / 75) * mouseMotion.Relative.Y * MouseSensitivity;

                // Clamp pitch to prevent flipping
                pitch = Mathf.Clamp(pitch, -Mathf.Pi / 2, Mathf.Pi / 2-.001f);

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
                    GD.Print("colliding!");
                    globalVars.isHover = true;
                    Node3D collider = result["collider"].As<Node3D>();

                    IHoverable colliderhoverable = (IHoverable)collider.GetParentNode3D();
                    if(!tracking){
                        highlightingStar = (IHoverable)collider.GetParentNode3D();
                    }
                    globalVars.hoverLabel = colliderhoverable.GetHoverText();

                    
                    // if (!String.IsNullOrWhiteSpace(star.starName)){
                    //     globalVars.hoverLabel = $"{star.starName}\nHIP {star.hipID}";
                    // }else{
                    //     globalVars.hoverLabel = $"Unnamed Star\nHIP {star.hipID}";
                    // }
                }
                else
                {
                    globalVars.isHover = false;
                    if(!tracking){
                        highlightingStar = null;
                    }
                }
            }
            if(middleMouseClicked && globalVars.isHover){
                GD.Print("I'm tracking you now!");
                ScreenOffset = UnprojectPosition(highlightingStar.GetGlobalTransform().Origin);
                middleMouseClicked = false;
                tracking = !tracking;
            }
        }

        private void ZoomIn()
        {
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
            float RotationSpeed = 1f;
            // Check if the 'screenshot_key' action is pressed
            if (Input.IsActionJustPressed("screenshot_key"))
            {
                TakeScreenshot();
            }
            if (highlightingStar == null || !tracking)
                return;


            // Compute the direction to the target
            Vector3 toTarget = (highlightingStar.GetGlobalTransform().Origin - GlobalTransform.Origin).Normalized();

            // Construct a rotation basis that maintains a fixed up vector (prevents roll)
            Basis newBasis = new Basis();
            newBasis.Z = -toTarget; // Forward vector points towards target
            newBasis.Y = Vector3.Up; // Fix up vector to avoid roll
            newBasis.X = newBasis.Y.Cross(newBasis.Z).Normalized(); // Right vector
            newBasis.Y = newBasis.Z.Cross(newBasis.X).Normalized(); // Ensure orthogonality

            // Apply yaw and pitch separately
            Transform3D newTransform = GlobalTransform;
            newTransform.Basis = newBasis;


            // Assign the new transform
            GlobalTransform = newTransform;
            GD.Print($"{yaw} and {Rotation.X}\n{pitch} and {Rotation.Y}");
        }
        private void TakeScreenshot()
        {
            // Get the current viewport as an Image
            Viewport viewport = GetNode<Viewport>("/root/Control/SubViewport2");
            Image screenshotImage = viewport.GetTexture().GetImage();

            // Save the screenshot as a JPEG
            screenshotImage.SavePng(screenshotPath);
            GD.Print($"Screenshot saved to {screenshotPath}");
        }
    }
}