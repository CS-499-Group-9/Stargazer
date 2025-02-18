using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node3D
{
	[Export] public PackedScene StarScene {get; set;}
	[Export] public PackedScene LabelScene {get; set;}
	
	private readonly float[,] starPos = {{0, 45, 1}, {10, 45, 2}, {20, 45, 3}, {30, 45, 4}, {40, 45, 5}, {50, 45, 6}};
	private readonly string[,] constLines = { { "s1", "s2" }, { "s2", "s3" }, { "s3", "s4" }, { "s4", "s5" } };
	
	private Globals globalVars;
	private List<Star> stars;
	private Boolean constDrawn = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		stars = new List<Star>();
		globalVars = GetNode<Globals>("/root/Globals");
		Random rnd = new Random();
		for (int i = 0; i < starPos.GetLength(0); i++)
		{
			stars.Add(SpawnStar(starPos[i, 0], starPos[i, 1], starPos[i,2], $"s{i+1}"));
		}
		for (int i = 0; i < 2500; i++){
			stars.Add(SpawnStar((float)rnd.NextDouble() * 360, (float)rnd.NextDouble() * 90, rnd.Next(1, 7), "s"));
		}
		for (int i = 0; i < 2500; i++){
			stars.Add(SpawnStar((float)rnd.NextDouble() * 360, (float)rnd.NextDouble() * -90, rnd.Next(1, 7), "s"));
		}
		DrawConstellation(constLines);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Triggers only once to save on performace.
		if (globalVars.isConstellation && !constDrawn)
		{
			DrawConstellation(constLines);
			constDrawn = true;
		}
		else if (!globalVars.isConstellation && constDrawn)
		{
			foreach (var child in GetChildren())
			{
				if (child is MeshInstance3D || (child is Node3D label && label.Name == ("Label")))
					child.QueueFree(); // Remove the constellation line meshes
			}
			constDrawn = false;
		}
	}
	
	private void DrawConstellation(string[,] lines)
	{
		MeshInstance3D constMesh = new MeshInstance3D();
		ImmediateMesh mesh = new ImmediateMesh();
		Vector3 labelPos = new Vector3();
		Vector3 totalPos = new Vector3(0, 0, 0);
		int c = 0;
		mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
		
		// Create a white material
		StandardMaterial3D whiteMaterial = new StandardMaterial3D();
		whiteMaterial.AlbedoColor = new Color(1, 1, 1); // White color

		// Assign the material to the mesh
		constMesh.MaterialOverride = whiteMaterial;
		
		for (int i = 0; i < lines.GetLength(0); i++)
		{
			Star s1 = stars.Find(x => x.starName == lines[i, 0]);
			Star s2 = stars.Find(x => x.starName == lines[i, 1]);
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
		mesh.SurfaceEnd();
		constMesh.Mesh = mesh;
		AddChild(constMesh);
		
		// Creating labels
		labelPos = totalPos / c;
		Node3D labelNode = LabelScene.Instantiate<Node3D>();
		labelNode.Position = labelPos;
		AddChild(labelNode);
	}
	
	private Star SpawnStar(float azimuth, float altitude, float magnitude, string name){
		Star star = StarScene.Instantiate<Star>();
		star.azimuth = azimuth;
		star.altitude = altitude;
		star.mag = magnitude;
		star.starName = name;
		AddChild(star);
		return star;
	}
}
