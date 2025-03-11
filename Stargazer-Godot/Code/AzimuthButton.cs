using Godot;
using System;

public partial class AzimuthButton : Control
{
    public Action<bool> GridlinesToggled;

    private void ToggleAzimuth(bool state)
    {
        GridlinesToggled(state);
    }
}
