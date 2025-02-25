using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Spawner : Node3D
{
	[Export] public PackedScene StarScene {get; set;}
	[Export] public PackedScene LabelScene {get; set;}
	
	// This is for building our sample constellation
	private readonly float[,] starPos = {{0, 45, 1}, {10, 45, 2}, {20, 45, 3}, {30, 45, 4}, {40, 45, 5}, {50, 45, 6}};
	private readonly string[,] constLines = { { "s1", "s2" }, { "s2", "s3" }, { "s3", "s4" }, { "s4", "s5" } };
	
	private Globals globalVars;
	private List<Star> stars;
	private List<Node3D> labels;
	private Boolean constDrawn = true;
	private Boolean labelDrawn = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		stars = new List<Star>(); // Star list is empty on scene startup
		globalVars = GetNode<Globals>("/root/Globals"); // Import globals
		Random rnd = new Random();
		// Draw stars...
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
		// ... then draw constellations
		DrawConstellation(constLines);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Triggers only once to save on performance.
		if (globalVars.isConstellation && !constDrawn)
		{
			DrawConstellation(constLines);
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
	
	private void DrawConstellation(string[,] lines)
	{
		labels = new List<Node3D>();
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
		labelNode.Visible = labelDrawn;
		labels.Add(labelNode);
		labels.ForEach((label) => { AddChild(label); });
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
