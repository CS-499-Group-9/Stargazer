using Godot;
using System;

public partial class ScreenshotButton : Button
{
    public override void _Ready()
    {
        Connect("pressed", new Callable(this, nameof(OnButtonPressed)));
    }

    private void OnButtonPressed()
    {
        GD.Print("Screenshot button pressed, emitting global signal.");
        
        // Emit the signal using the instance of GlobalSignals
        Globals.Instance.EmitSignal("ScreenshotRequested");
    }
}
