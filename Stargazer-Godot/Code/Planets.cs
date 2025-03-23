using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Planets : Node3D
{
	[Export] PackedScene PlanetScene { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
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
			AddChild(newPlanet);
		}
	}


}
