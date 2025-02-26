using Godot;
using System;

public partial class ConstellationLabel : Label3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parent = GetParent<LabelNode>();
		Text = parent.LabelText;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
