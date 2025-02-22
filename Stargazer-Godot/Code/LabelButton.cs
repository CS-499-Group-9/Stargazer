using Godot;
using System;

public partial class LabelButton : Control
{
    private Globals globalVars;
    public override void _Ready()
    {
        globalVars = GetNode<Globals>("/root/Globals");
    }

    private void ToggleLabel(bool state)
    {
        globalVars.isLabel = state;
    }
}
