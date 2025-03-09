using DataLayer;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public partial class Spawner2D : Node2D
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
		while(!globalVars.populated){
			await Task.Delay(100);
		}
        GD.Print("2d turn!");
		dataPackage = globalVars.dataPackage;
        starProducer = dataPackage.Stars;
        constellations = dataPackage.Constellations;
        messierProducer = dataPackage.MessierObjects;
		
        while (!starProducer.IsCompleted)
        {
            foreach (var star in starProducer.GetConsumingEnumerable())
            {
				SpawnStar(star);
				
            }
			Thread.Sleep(10);
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
        GD.Print($"That took idk seconds.");

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
			foreach (Node2D child in GetChildren())
			{
				if (child is MeshInstance2D)
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

		MeshInstance2D constMesh = new MeshInstance2D();
		Vector2 labelPos = new Vector2();
		ImmediateMesh mesh = new ImmediateMesh();
		// Create a white material
		StandardMaterial3D whiteMaterial = new StandardMaterial3D();
		whiteMaterial.AlbedoColor = new Color(0.8f, 0.8f, 0.8f, 0.8f); // White color
		whiteMaterial.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
		// Assign the material to the mesh
		constMesh.Material = whiteMaterial;
		mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
		labels = new List<LabelNode>();

        foreach (var constellation in constellations)
        { 
			Vector2 totalPos = new Vector2(0, 0);
			int c = 0;
			//GD.Print($"Drawing constellation {constellation.ConstellationName}");

			foreach(var lines in constellation.ConstellationLines) {

				Star s1 = dataPackage.GetConstellationStar(lines.Item1, SpawnStar);
				Star s2 = dataPackage.GetConstellationStar(lines.Item2, SpawnStar);
				if (s1.altitude >= 0 || s2.altitude >= 0){
					mesh.SurfaceAddVertex(new Vector3(s1.Pos2D.X,s1.Pos2D.Y,0));
					mesh.SurfaceAddVertex(new Vector3(s2.Pos2D.X,s2.Pos2D.Y,0));
					GD.Print("Vec");
					GD.Print(new Vector3(s1.Pos2D.X,s1.Pos2D.Y,0));
				}
				if (totalPos == Vector2.Zero) // solely checked for the first star
				{
					totalPos += s1.Pos2D;
					c++;
				}
				totalPos += s2.Pos2D;
				c++;
			}
		
			// Creating labels
			labelPos = totalPos / c;
		
			LabelNode labelNode = LabelScene.Instantiate<LabelNode>();
			labelNode.LabelText = constellation.ConstellationName;
			labelNode.Position = new Vector3(labelPos.X,labelPos.Y,0);
			labelNode.Visible = labelDrawn;
			labels.Add(labelNode);
        }
		mesh.SurfaceEnd();
		constMesh.Mesh = mesh;
		AddChild(constMesh);
		//labels.ForEach((label) => { AddChild(label); });
	
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
