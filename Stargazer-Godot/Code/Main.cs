using System.Threading.Tasks;
using Godot;

public partial class Main : Camera3D
{
    [Export] public float MouseSensitivity = 0.002f;
    [Export] public Camera3D ScreenshotCam;
    [Export] public Viewport ScreenshotViewport;
    
    private float yaw = 0f;
    private float pitch = 0f;
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
            pitch = Mathf.Clamp(pitch, 0, Mathf.Pi / 2);
            Rotation = new Vector3(pitch, yaw, 0);
        }
    }

    public override async void _Process(double delta)
    {
        if (Input.IsActionJustPressed("screenshot_key"))
        {
            await TakeScreenshot();
        }
    }

    private async Task TakeScreenshot()
    {
        if (ScreenshotCam == null || ScreenshotViewport == null)
        {
            GD.Print("No alternative camera or viewport assigned!");
            return;
        }

        // **Step 1: Assign the Screenshot Camera to the Separate Viewport**
        ScreenshotViewport.World3D = GetViewport().World3D;
        ScreenshotCam.Current = true;

        // **Step 2: Wait for the Next Frame to Ensure the Scene Updates**
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        // **Step 3: Capture the Screenshot Without Affecting the Main Viewport**
        Image screenshotImage = ScreenshotViewport.GetTexture().GetImage();
        screenshotImage.SaveJpg(screenshotPath);

        // **Step 4: Restore the Main Camera**
        ScreenshotCam.Current = false;

        GD.Print($"Screenshot saved to {screenshotPath}");
    }
}