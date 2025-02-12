using System.Threading.Tasks;
using Godot;

public partial class Main : Camera3D
{
    [Export] public float MouseSensitivity = 0.002f;
    [Export] public Camera3D ScreenshotCam;
    private float yaw = 0f;  // Left/Right Rotation
    private float pitch = 0f; // Up/Down Rotation
    private bool rightClickHeld = false;
    private string screenshotPath = "user://screenshot.jpeg";

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
	public override void _Process(double delta)
    {
        // Check if the 'screenshot_key' action is pressed
        if (Input.IsActionJustPressed("screenshot_key"))
        {
            TakeScreenshot();
        }
    }

    private async Task TakeScreenshot()
    {
        if (ScreenshotCam == null)
        {
            GD.Print("No alternative camera assigned!");
            return;
        }

        // Get the current viewport as an Image
        Camera3D originalCam = GetViewport().GetCamera3D();

        ScreenshotCam.Current = true;
        await ToSignal(GetTree(), "process_frame");
        Viewport viewport = GetViewport();

        Image screenshotImage = viewport.GetTexture().GetImage();

        // Save the screenshot as a JPEG
        screenshotImage.SaveJpg(screenshotPath);
        originalCam.Current = true;
        GD.Print($"Screenshot saved to {screenshotPath}");
    }
}