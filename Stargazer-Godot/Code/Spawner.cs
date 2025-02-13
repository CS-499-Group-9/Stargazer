using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node3D
{
	[Export] public PackedScene StarScene {get; set;}
	
	private readonly float[,] starPos = {{0, 45, 1}, {10, 45, 2}, {20, 45, 3}, {30, 45, 4}, {40, 45, 5}, {50, 45, 6}};
	private readonly string[,] cnst_lines = { { "s1", "s2" }, { "s2", "s3" }, { "s3", "s4" }, { "s4", "s5" } };
	
	private Globals globalVars;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var stars = new List<Star>();
		globalVars = GetNode<Globals>("/root/Globals");
		
		for (int i = 0; i < starPos.GetLength(0); i++)
		{
			stars.Add(SpawnStar(starPos[i, 0], starPos[i, 1], starPos[i,2], $"s{i+1}"));
		}
		/*for (int i = 0; i < 2500; i++){
			stars.Add(SpawnStar((float)rnd.NextDouble() * 360, (float)rnd.NextDouble() * 90, rnd.Next(1, 7)));
		}
		for (int i = 0; i < 2500; i++){
			stars.Add(SpawnStar((float)rnd.NextDouble() * 360, (float)rnd.NextDouble() * -90, rnd.Next(1, 7)));
		}*/
		
		DrawConstellation(stars, cnst_lines);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void DrawConstellation(List<Star> stars, string[,] lines)
	{
		MeshInstance3D constMesh = new MeshInstance3D();
		ImmediateMesh mesh = new ImmediateMesh();
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
			GD.Print("Star 1 Name: " + s1.starName);
			GD.Print("Star 2 Name: " + s2.starName);
			mesh.SurfaceAddVertex(s1.Position);
			mesh.SurfaceAddVertex(s2.Position);
		}
		mesh.SurfaceEnd();
		constMesh.Mesh = mesh;
		AddChild(constMesh);
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
