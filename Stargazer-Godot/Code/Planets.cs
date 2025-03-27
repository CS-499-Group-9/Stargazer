using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Planets : Node3D
{
	[Export] ResourcePreloader planetTextures;
	[Export] PackedScene PlanetScene { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//ResourcePreloader planetTextures = GetNode<ResourcePreloader>("PlanetTextureLoader");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void DrawPlanets(IEnumerable<HorizonalPlanet> planets, IPlanetaryCalculator<HorizonalPlanet> planetaryCalculator)
	{
		foreach(var child in GetChildren()) {child.Free(); }

		foreach (var planet in planets)
		{
			var newPlanet = PlanetScene.Instantiate<Planet>();
			newPlanet.FromHorizontal(planet, planetaryCalculator);
			newPlanet.setTexture(GD.Load<Texture2D>($"res://Textures/Resources/{planet.Name.ToLower()}map.tres"));
			AddChild(newPlanet);
		}
	}


}
