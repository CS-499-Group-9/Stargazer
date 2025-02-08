using Godot;
using System;

public partial class AzimuthButton : Control
{
    private Globals globalVars;
    public override void _Ready()
    {
        globalVars = GetNode<Globals>("/root/Globals");
    }

    private void ToggleAzimuth(bool state)
    {
        globalVars.isAzimuth = state;
        GD.Print(globalVars.isAzimuth);
    }
}
