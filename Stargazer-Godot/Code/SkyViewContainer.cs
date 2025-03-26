using Godot;
using Stargazer;
using System;

public partial class SkyViewContainer : SubViewportContainer
{
	public SkyView SkyView { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		SkyView = GetNode<SkyView>("SubViewport/SkyView");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
