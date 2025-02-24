using Godot;

public partial class ScreenshotButton : Button
{
    private Main camera; // Ensure this matches your Camera3D script class name

    public override void _Ready()
    {
        camera = GetTree().Root.FindChild("Camera3D", true, false) as Main;

        if (camera == null)
        {
            GD.PrintErr("Camera not found!");
        }
    }

    public async void OnButtonPressed()
    {
        if (camera != null)
        {
           await camera.TakeScreenshot();
        }
        else
        {
            GD.PrintErr("Cannot take screenshot: Camera is null!");
        }
    }
}
