using Godot;
using System;

public partial class LabelButton : Control
{
    public Action<bool> ConstellationLabelsToggled;
    public override void _Ready()
    {
    }

    private void ToggleLabel(bool state)
    {
        ConstellationLabelsToggled(state);
    }
}
