using Godot;

public partial class Main : Camera3D
{
    [Export] public float MouseSensitivity = 0.002f;
    private float yaw = 0f;  // Left/Right Rotation
    private float pitch = 0f; // Up/Down Rotation
    private bool rightClickHeld = false;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Right)
            {
                rightClickHeld = mouseButton.Pressed;
                Input.MouseMode = rightClickHeld ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
            }
        }

        if (rightClickHeld && @event is InputEventMouseMotion mouseMotion)
        {
            yaw -= mouseMotion.Relative.X * MouseSensitivity;
            pitch -= mouseMotion.Relative.Y * MouseSensitivity;

            // Clamp pitch to prevent flipping
            pitch = Mathf.Clamp(pitch, 0, Mathf.Pi / 2);

            // Apply rotation
            Rotation = new Vector3(pitch, yaw, 0);
        }
    }
}