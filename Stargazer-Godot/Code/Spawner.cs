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
		HuntsvilleCoordinates huntsvilleCoordinates = new HuntsvilleCoordinates();
		var provider = InjectionService.GetServiceProvider(ProjectSettings.GlobalizePath("res://"));
		var repoService = provider.GetRequiredService<StargazerRepositoryService>();
		repoService.CalculateStars(huntsvilleCoordinates.lattitude, huntsvilleCoordinates.longitude, DateTime.UtcNow);
		var starProducer = repoService.GetStars();
		while (!starProducer.IsCompleted)
		{
            foreach (var item in starProducer.GetConsumingEnumerable())
            {
				Star star = StarScene.Instantiate<Star>();
				star.azimuth = (float)item.Azimuth;
				star.altitude = (float)item.Altitude;
				star.mag = (float)item.Magnitude;
				AddChild(star);
            }
        }
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

	private struct HuntsvilleCoordinates
	{
		public double lattitude = 34.7304;
		public double longitude = -86.5861;
        public HuntsvilleCoordinates()
        {
        }
    }
}
