using Godot;
using Stargazer;
using System;

public partial class MainContainer : VBoxContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Startup GetStartup() { return GetNode<Startup>("InteractionContainer"); }
}
