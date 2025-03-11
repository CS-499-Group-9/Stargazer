using Godot;
using System;

public partial class AzimuthButton : Control
{
    public Action<bool> GridlinesToggled;
    private Globals globalVars;
    public override void _Ready()
    {
        globalVars = GetNode<Globals>("/root/Globals");
    }

    private void ToggleAzimuth(bool state)
    {
        GridlinesToggled(state);
    }
}
