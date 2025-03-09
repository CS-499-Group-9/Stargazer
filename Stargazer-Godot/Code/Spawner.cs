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
	
	private Globals globalVars ;
	private List<LabelNode> labels;
	private Boolean constDrawn = true;
	private Boolean labelDrawn = true;

	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        globalVars = GetNode<Globals>("/root/Globals"); // Import globals


		// ... then draw constellations
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Triggers only once to save on performance.
		if (globalVars.isConstellation && !constDrawn)
		{
			//DrawConstellations();
			constDrawn = true;
		}
		else if (!globalVars.isConstellation && constDrawn)
		{
			foreach (Node3D child in GetChildren())
			{
				if (child is MeshInstance3D)
				{
					child.QueueFree();
				} // Remove the constellation line meshes
			}
			constDrawn = false;
		}
		
		// Triggers only once to save on performance.
		if (globalVars.isLabel && !labelDrawn && constDrawn)
		{
			foreach (var label in labels)
			{
				label.Visible = true;
			}
			labelDrawn = true;
		}
		else if ((!globalVars.isLabel && labelDrawn) || !constDrawn)
		{
			foreach (var label in labels)
			{
				label.Visible = false;
			}
			labelDrawn = false;
		}
	}

	public  void DrawStars(Object source, CelestialDataPackage<Star> dataPackage)
	{
		var starProducer = dataPackage.Stars;
        while (!starProducer.IsCompleted)
        {
            foreach (var star in starProducer.GetConsumingEnumerable())
            {
                SpawnStar(star);
            }
        }

        DrawConstellations(dataPackage);
		var messierProducer = dataPackage.MessierObjects;
        while (!messierProducer.IsCompleted)
        {
            foreach (var item in messierProducer.GetConsumingEnumerable())
            {
                GD.Print($"Messier: {item.MessierId} {item.Type}");
            }
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
