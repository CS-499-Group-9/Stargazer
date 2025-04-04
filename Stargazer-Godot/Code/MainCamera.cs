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
        private bool trackObjectRequested = false;
        private string screenshotPath = "user://screenshot.jpeg";

        // Objects used for ray tracing and hovering/tracking
        private PhysicsDirectSpaceState3D worldSpace;
        private Vector2 mousePosition;
        private Vector3 rayStart, rayEnd;
        private ZoomState zoomState;
        private ITrackable TrackedBody { get; set; }
        private IHoverable HoverBody { get; set; }
  

        /// <inheritdoc/>
        public Action<float> OnZoom { get; set; }
        ///<inheritdoc/>
        public Action<ZoomState> OnZoomStateChanged { get ; set ; }
        /// <inheritdoc/>
        public Action<Camera3D> OnRotation { get ; set; }
        ///<inheritdoc/>
        public Action<Godot.Collections.Array<Plane>> OnFustrumChanged { get ; set ; }
        /// <inheritdoc/>
        public Action<IHoverable> OnHoverableChange { get ; set ; }
        /// <inheritdoc/>
        public override void _Ready()
        {
            worldSpace = GetWorld3D().DirectSpaceState;
        }

        /// <summary>
        /// Checks if the right mouse button is being held down to pan the view.
        /// </summary>
        /// <param name="event">The event data passed in.</param>
        public override void _Input(InputEvent @event)
        {
            trackObjectRequested = false;
            if(@event.IsAction("forward")){
                Position -= 0.5f * Basis.Z;
            }


            if (@event is InputEventMouseButton mouseButton)
            {
                switch (mouseButton.ButtonIndex)
                {
                    case MouseButton.Right:
                        // User has requested to pan/tilt (or is finished requesting a pan/tilt)
                        rightClickHeld = mouseButton.Pressed;
                        Input.MouseMode = rightClickHeld ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
                        break;
                    case MouseButton.Left:
                        if(!mouseButton.Pressed) break;
                        // User has requested a change to object tracking.
                        if (HoverBody is not null)
                        {
                            // User has clicked on an object and requests to track it.
                            TrackedBody = HoverBody;
                            //ScreenOffset = UnprojectPosition(TrackedBody.GlobalTransform.Origin);
                            trackObjectRequested = false;
                        }
                        else if (TrackedBody is not null)
                        {
                            // User has clicked away from a body and requests to discontinue tracking
                            yaw = Rotation.Y;
                            pitch = Rotation.X;
                            TrackedBody = null;
                        }
                        break;
                    case MouseButton.Middle:
                        // Not currently used.
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
                if (rightClickHeld && TrackedBody is null)
                {
                    // User is attempting to pan/tilt the view
                    yaw -= (Fov / 75) * mouseMotion.Relative.X * MouseSensitivity;
                    pitch -= (Fov / 75) * mouseMotion.Relative.Y * MouseSensitivity;

                    // Clamp pitch to prevent flipping
                    pitch = Mathf.Clamp(pitch, -Mathf.Pi / 2, Mathf.Pi / 2 - .001f);

                    // Apply rotation
                    Rotation = new Vector3(pitch, yaw, 0);

                    // Notify subscribers of the change.
                    OnRotation?.Invoke(this);
                }
            }
            
        }

        private void ZoomIn()
        {
            // Decrease field of view for zooming in (if using a perspective camera)
            Fov = Mathf.Clamp(Fov - 2, 10, 90); // Example: Adjust sensitivity (2) and clamp the FOV

            // Check if Fov has passed through a threshold and update the Zoom State as necessary and notify subscribers of the change
            if (Fov < 15 && zoomState <= ZoomState.FullIn) { OnZoomStateChanged?.Invoke(zoomState = ZoomState.FullIn); }
            if (Fov < 35 && zoomState <= ZoomState.Middle) { OnZoomStateChanged?.Invoke(zoomState = ZoomState.Middle); }
            OnZoom?.Invoke(Fov);
        }

        private void ZoomOut()
        {
            // Decrease field of view for zooming in (if using a perspective camera)
            Fov = Mathf.Clamp(Fov + 2, 10, 90); // Example: Adjust sensitivity (2) and clamp the FOV

            // Check if Fov has passed through a threshold and update the Zoom State as necessary and notify subscribers of the change
            if (Fov > 35 && zoomState >= ZoomState.FullOut) { OnZoomStateChanged?.Invoke(zoomState = ZoomState.FullOut); }
            if (Fov > 15 && zoomState >= ZoomState.Middle) { OnZoomStateChanged?.Invoke(zoomState = ZoomState.Middle); }
            OnZoom?.Invoke(Fov);
        }


        /// <summary>
        /// Tracks a body (if armed) and implements ray tracing to hover over bodies.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
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

                // Notify subscribers the rotation has changed.
                OnRotation?.Invoke(this);


                // Assign the new transform
            }

            if (rightClickHeld) return; // Disables ray tracing when pan/tilt is active.

            // Ray tracing to detect when the mouse is pointed at an object.
            mousePosition = GetViewport().GetMousePosition();
            rayStart = ProjectRayOrigin(mousePosition);
            rayEnd = ProjectPosition(mousePosition, 1000);

            var result = worldSpace.IntersectRay(PhysicsRayQueryParameters3D.Create(rayStart, rayEnd));

            if (result.Count > 0 )
            {

                Node3D collider = result["collider"].As<Node3D>();
                try
                {
                    // Update the hover body reference and notify subscribers of the change.
                    HoverBody = (IHoverable)collider.GetParentNode3D();
                    OnHoverableChange?.Invoke(HoverBody);
                }
                catch (Exception)
                {
                    // Just in case there are other objects added that are not intended to be able to select.
                }
            }
            else if (HoverBody is not null) 
            {
                // Update the hover body reference and notify subscribers of the change.
                HoverBody = null;
                OnHoverableChange?.Invoke(null);
            }
            
        }


        
    }
}