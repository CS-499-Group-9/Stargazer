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
	
	// This is for building our sample constellation
	private readonly float[,] starPos = {{0, 45, 1}, {10, 45, 2}, {20, 45, 3}, {30, 45, 4}, {40, 45, 5}, {50, 45, 6}};
	private readonly string[,] constLines = { { "s1", "s2" }, { "s2", "s3" }, { "s3", "s4" }, { "s4", "s5" } };
	
	private Globals globalVars ;
	private List<LabelNode> labels;
	private Boolean constDrawn = true;
	private Boolean labelDrawn = true;

	CelestialDataPackage<Star> dataPackage;
	private BlockingCollection<HorizontalStar> starProducer;
	private BlockingCollection<HorizontalMessierObject> messierProducer;
	private IEnumerable<Constellation> constellations;
	
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
        globalVars = GetNode<Globals>("/root/Globals"); // Import globals
        Stopwatch sw = Stopwatch.StartNew();
        HuntsvilleCoordinates huntsvilleCoordinates = new HuntsvilleCoordinates();
        var repoService = await InjectionService<Star>.GetRepositoryServiceAsync(ProjectSettings.GlobalizePath("res://"));
        
		dataPackage = await repoService.UpdateUserPosition(huntsvilleCoordinates.lattitude, huntsvilleCoordinates.longitude, new DateTime(2025, 02, 28,12,00,00 ).ToUniversalTime());
        starProducer = dataPackage.Stars;
        constellations = dataPackage.Constellations;
        messierProducer = dataPackage.MessierObjects;
		
        while (!starProducer.IsCompleted)
        {
            foreach (var star in starProducer.GetConsumingEnumerable())
            {
				SpawnStar(star);
            }
        }
		
		DrawConstellations();
		
		while (!messierProducer.IsCompleted)
        {
            foreach (var item in messierProducer.GetConsumingEnumerable())
            {
                GD.Print($"Messier: {item.MessierId} {item.Type}");
            }
        }

        sw.Stop();
        GD.Print($"That took {sw.Elapsed.TotalSeconds} seconds.");

		// ... then draw constellations
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Triggers only once to save on performance.
		if (globalVars.isConstellation && !constDrawn)
		{
			DrawConstellations();
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


	
	private async void DrawConstellations()
	{

		MeshInstance3D constMesh = new MeshInstance3D();
		Vector3 labelPos = new Vector3();
		ImmediateMesh mesh = new ImmediateMesh();
		// Create a white material
		StandardMaterial3D whiteMaterial = new StandardMaterial3D();
		whiteMaterial.AlbedoColor = new Color(0.8f, 0.8f, 0.8f, 0.8f); // White color
		whiteMaterial.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
		// Assign the material to the mesh
		constMesh.MaterialOverride = whiteMaterial;
		mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
		labels = new List<LabelNode>();

        foreach (var constellation in constellations)
        { 
			Vector3 totalPos = new Vector3(0, 0, 0);
			int c = 0;
			GD.Print($"Drawing constellation {constellation.ConstellationName}");

			foreach(var lines in constellation.ConstellationLines) {

				Star s1 = dataPackage.GetConstellationStar(lines.Item1, SpawnStar);
				Star s2 = dataPackage.GetConstellationStar(lines.Item2, SpawnStar);
				mesh.SurfaceAddVertex(s1.Position);
				mesh.SurfaceAddVertex(s2.Position);
				if (totalPos == Vector3.Zero) // solely checked for the first star
				{
					totalPos += s1.Position;
					c++;
				}
				totalPos += s2.Position;
				c++;
			}
		
			// Creating labels
			labelPos = totalPos / c;
		
			LabelNode labelNode = LabelScene.Instantiate<LabelNode>();
			labelNode.LabelText = constellation.ConstellationName;
			labelNode.Position = labelPos;
			labelNode.Visible = labelDrawn;
			labels.Add(labelNode);
        }
		mesh.SurfaceEnd();
		constMesh.Mesh = mesh;
		AddChild(constMesh);
		labels.ForEach((label) => { AddChild(label); });
	
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

    private struct HuntsvilleCoordinates
    {
        public double lattitude = 34.7304;
        public double longitude = -86.5861;
        public HuntsvilleCoordinates()
        {
        }
    }
}
