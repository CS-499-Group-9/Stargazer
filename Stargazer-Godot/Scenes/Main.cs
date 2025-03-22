using Godot;
using System;

public partial class Main : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var mainContainer = GetNode<MainContainer>(nameof(MainContainer));
		var view2d = GetNode<>
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
