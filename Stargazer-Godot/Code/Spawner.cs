using Godot;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public partial class Spawner : Node3D
{
	[Export] public PackedScene StarScene {get; set;}
	private readonly float[,] starPos = {{0, 45, 1}, {10, 45, 2}, {20, 45, 3}, {30, 45, 4}, {40, 45, 5}, {50, 45, 6}};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private Star SpawnStar(float azimuth, float altitude, float magnitude){
		Star star = StarScene.Instantiate<Star>();
		star.azimuth = azimuth;
		star.altitude = altitude;
		star.mag = magnitude;
		AddChild(star);
		return star;
	}
}
