using Godot;
using Stargazer;
using System;

public partial class Label : Godot.Label
{
    private Globals globalVars;
    public override void _Ready()
    {
        globalVars = GetNode<Globals>("/root/Globals"); // Import globals
        this.Visible = globalVars.isHover;
    }
    
    public override void _Process(double delta)
    {
        this.Visible = globalVars.isHover;
        this.SetText(globalVars.hoverLabel);
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
