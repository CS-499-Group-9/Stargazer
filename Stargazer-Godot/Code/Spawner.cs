using DataLayer;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Spawner : Node3D
{
	[Export] public PackedScene StarScene {get; set;}
	[Export] public PackedScene LabelScene {get; set;}

	private Startup Startup;
	private List<LabelNode> labels;
	private Boolean constDrawn = true;
	private Boolean labelDrawn = true;

	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		// ... then draw constellations
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Triggers only once to save on performance.
		
	}

	public  void DrawStars(Object source, CelestialDataPackage<Star> dataPackage)
	{
		foreach (var s in GetChildren()) { s.Free(); }
		var starProducer = dataPackage.Stars;
        foreach (var star in starProducer)
        {
			SpawnStar(star);
        }
        
		var messierProducer = dataPackage.MessierObjects;
        foreach (var item in messierProducer)
        {
            GD.Print($"Messier: {item.MessierId} {item.Type}");
        }
        
    }
	

	private Star SpawnStar(HorizontalStar horizontalStar){
		Star star = StarScene.Instantiate<Star>();
		star.azimuth = (float)horizontalStar.Azimuth;
		star.altitude = (float)horizontalStar.Altitude;
		star.mag = (float)horizontalStar.Magnitude;
		star.starName = horizontalStar.StarName;

		AddChild(star);
		return star;
	}

    
}
