using Godot;
using System;

public partial class ScreenshotView : SubViewport
{
	private string screenshotPath = "user://screenshot.jpeg";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
