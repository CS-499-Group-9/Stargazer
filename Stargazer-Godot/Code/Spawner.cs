using Godot;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

public partial class Spawner : Node3D
{
	[Export] public PackedScene StarScene {get; set;}
	
	private readonly float[,] starPos = {{0, 45, 1}, {10, 45, 2}, {20, 45, 3}, {30, 45, 4}, {40, 45, 5}, {50, 45, 6}};
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		Stopwatch sw = Stopwatch.StartNew();
		HuntsvilleCoordinates huntsvilleCoordinates = new HuntsvilleCoordinates();
		var repoService = await InjectionService<Star>.GetRepositoryServiceAsync(ProjectSettings.GlobalizePath("res://"));
		
		var dataPackage = repoService.CalculateHorizontalObjects(huntsvilleCoordinates.lattitude, huntsvilleCoordinates.longitude, DateTime.UtcNow);
		var starProducer = dataPackage.Stars;
		var constellationStars = dataPackage.ConstellationStars;
		var constellations = dataPackage.Constellations;
		var drawnStars = dataPackage.DrawnStars;

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
		var messierProducer = dataPackage.MessierObjects;
		while (!messierProducer.IsCompleted)
		{
			foreach (var item in messierProducer.GetConsumingEnumerable())
			{
				GD.Print($"Messier: {item.MessierId} {item.Type}");
			}
		}

		sw.Stop();
		GD.Print($"That took {sw.Elapsed.TotalSeconds} seconds.");
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
