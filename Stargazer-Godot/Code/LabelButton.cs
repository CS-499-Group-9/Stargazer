using Godot;
using System;

public partial class LabelButton : Control
{
    private Globals globalVars;
    private EventHandler<bool> Clicked;
    public override void _Ready()
    {
        var startup = GetParent<Startup>();
        Clicked = startup.ToggleConstellationNames;
    }

    private void ToggleLabel(bool state)
    {
        Clicked?.Invoke(this, state);
    }
}
