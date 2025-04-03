using Godot;
using System;
using System.Runtime.CompilerServices;

namespace Stargazer
{
    /// <summary>
    /// Contains the main camera used to create the user view.
    /// </summary>
    public partial class MainCamera : Camera3D, ICameraStateNotifier
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
        private PhysicsDirectSpaceState3D worldSpace;
        private Vector2 mousePosition;
        private Vector3 rayStart, rayEnd;


        private ZoomState zoomState;
        private ITrackable trackedObject;
  
        private Globals globalVars;

        public Action<float> OnZoom { get; set; }
        public Action<ZoomState> OnZoomStateChanged { get ; set ; }

        public ITrackable TrackedBody { get; private set; }

        public IHoverable HoverBody { get; private set; }
        public Action<Camera3D> OnRotation { get ; set; }
        public Action<Godot.Collections.Array<Plane>> OnFustrumChanged { get ; set ; }
        public Action<IHoverable> OnHoverableChange { get ; set ; }


        /// <inheritdoc/>
        public override void _Ready()
        {
            globalVars = GetNode<Globals>("/root/Globals"); // Import globals
            worldSpace = GetWorld3D().DirectSpaceState;
            
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
                Position -= 0.5f * Basis.Z;

            }


            if (@event is InputEventMouseButton mouseButton)
            {
                switch (mouseButton.ButtonIndex)
                {
                    case MouseButton.Right:
                        rightClickHeld = mouseButton.Pressed;
                        Input.MouseMode = rightClickHeld ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
                        break;
                    case MouseButton.Left:
                        middleMouseClicked = mouseButton.Pressed;

                        break;
                    case MouseButton.Middle:
                        break;
                    case MouseButton.WheelUp:
                        ZoomIn();
                        break;
                    case MouseButton.WheelDown:
                        ZoomOut();
                        break;
                }

            } else if (@event is InputEventMouseMotion mouseMotion)
            {
                if (rightClickHeld && trackedObject is null)
                {
                    yaw -= (Fov / 75) * mouseMotion.Relative.X * MouseSensitivity;
                    pitch -= (Fov / 75) * mouseMotion.Relative.Y * MouseSensitivity;

                    // Clamp pitch to prevent flipping
                    pitch = Mathf.Clamp(pitch, -Mathf.Pi / 2, Mathf.Pi / 2 - .001f);

                    // Apply rotation
                    Rotation = new Vector3(pitch, yaw, 0);
                    OnRotation?.Invoke(this);
                }
            }

            if (!middleMouseClicked) return;
            if(HoverBody is not null){
                GD.Print("I'm tracking you now!");
                TrackedBody = (ITrackable)HoverBody;
                //ScreenOffset = UnprojectPosition(TrackedBody.GlobalTransform.Origin);
                middleMouseClicked = false;
                tracking = true;
            } else { TrackedBody = null; }
            
        }

        private void ZoomIn()
        {
            // Decrease field of view for zooming in (if using a perspective camera)
            Fov = Mathf.Clamp(Fov - 2, 10, 90); // Example: Adjust sensitivity (2) and clamp the FOV
            if (Fov < 15 && zoomState <= ZoomState.FullIn) { OnZoomStateChanged?.Invoke(zoomState = ZoomState.FullIn); }
            if (Fov < 35 && zoomState <= ZoomState.Middle) { OnZoomStateChanged?.Invoke(zoomState = ZoomState.Middle); }
            OnZoom?.Invoke(Fov);
        }

        private void ZoomOut()
        {
            // Decrease field of view for zooming in (if using a perspective camera)
            Fov = Mathf.Clamp(Fov + 2, 10, 90); // Example: Adjust sensitivity (2) and clamp the FOV
            if (Fov > 35 && zoomState >= ZoomState.FullOut) { OnZoomStateChanged?.Invoke(zoomState = ZoomState.FullOut); }
            if (Fov > 15 && zoomState >= ZoomState.Middle) { OnZoomStateChanged?.Invoke(zoomState = ZoomState.Middle); }
            OnZoom?.Invoke(Fov);
        }


        /// <summary>
        /// Used to check for the input for the screenshot key and take the screenshot.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            float RotationSpeed = 1f;
            // Check if the 'screenshot_key' action is pressed
            

            if (TrackedBody is not null)
            {
                // Compute the direction to the target
                Vector3 toTarget = (TrackedBody.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();

                // Construct a rotation basis that maintains a fixed up vector (prevents roll)
                Basis newBasis = new Basis();
                newBasis.Z = -toTarget; // Forward vector points towards target
                newBasis.Y = Vector3.Up; // Fix up vector to avoid roll
                newBasis.X = newBasis.Y.Cross(newBasis.Z).Normalized(); // Right vector
                newBasis.Y = newBasis.Z.Cross(newBasis.X).Normalized(); // Ensure orthogonality

                // Apply yaw and pitch separately
                Transform3D newTransform = GlobalTransform;
                newTransform.Basis = newBasis;
                GlobalTransform = newTransform;

                OnRotation?.Invoke(this);


                // Assign the new transform
            }
            mousePosition = GetViewport().GetMousePosition();
            rayStart = ProjectRayOrigin(mousePosition);
            rayEnd = ProjectPosition(mousePosition, 1000);
            if (rightClickHeld) return;
            var result = worldSpace.IntersectRay(PhysicsRayQueryParameters3D.Create(rayStart, rayEnd));
            if (result.Count > 0 )
            {
                globalVars.isHover = true;
                Node3D collider = result["collider"].As<Node3D>();

                HoverBody = (IHoverable)collider.GetParentNode3D();
                OnHoverableChange?.Invoke(HoverBody);
            }
            else if (HoverBody is not null) 
            {
                //.globalVars.isHover = false;
                HoverBody = null;
                OnHoverableChange?.Invoke(null);
            }
            
        }


        
    }
}