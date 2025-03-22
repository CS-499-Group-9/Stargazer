using Godot;
using Stargazer;
using System;

public partial class HoverLabel : Label
{
    private Globals globalVars;
    public override void _Ready()
    {
        globalVars = GetNode<Globals>("/root/Globals"); // Import globals
        Visible = globalVars.isHover;
        AddThemeFontSizeOverride("font_size", 25);
        LabelSettings.FontSize = 30;
    }
    
    public override void _Process(double delta)
    {
        Visible = globalVars.isHover;
        SetText(globalVars.hoverLabel);

    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            var shift = new Vector2(15f, 10f);
            Position = mouseMotion.Position + shift;
        }
    }
}
