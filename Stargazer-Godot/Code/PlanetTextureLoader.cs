using Godot;
using System;

public partial class PlanetTextureLoader : ResourcePreloader
{
	// Called when the node enters the scene tree for the first time.
	~PlanetTextureLoader(){
		GD.Print("the textuer loader is gone!");
	}
	public override void _Ready()
	{
		GD.Print("loaded up!");
		// Image mercuryImage = 
		// ImageTexture mercuryTexture = ImageTexture.CreateFromImage(Image.LoadFromFile("res://Textures/mercurymap.jpg"));
		// mercuryTexture.Load
		// mercuryTexture.GetImage("res://Textures/mercurymap.jpg");
		// AddResource("mercury",ImageTexture.CreateFromImage(Image.LoadFromFile("res://Textures/mercurymap.jpg")));
		// AddResource("mercury",GetResource())
		// AddResource("venus",ImageTexture.CreateFromImage(Image.LoadFromFile("res://Textures/venusmap.jpg")));
		// AddResource("mars",ImageTexture.CreateFromImage(Image.LoadFromFile("res://Textures/marsmap.jpg")));
		// AddResource("jupiter",ImageTexture.CreateFromImage(Image.LoadFromFile("res://Textures/jupitermap.jpg")));
		// AddResource("saturn",ImageTexture.CreateFromImage(Image.LoadFromFile("res://Textures/saturnmap.jpg")));
		// AddResource("uranus",ImageTexture.CreateFromImage(Image.LoadFromFile("res://Textures/uranusmap.jpg")));
		// AddResource("neptune",ImageTexture.CreateFromImage(Image.LoadFromFile("res://Textures/neptunemap.jpg")));
		// AddResource("sun",ImageTexture.CreateFromImage(Image.LoadFromFile("res://Textures/sunmap.jpg")));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
