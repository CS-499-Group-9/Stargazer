using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stargazer
{
	/// <summary>
	/// A <see cref="Node3D"/> container for the planets.
	/// </summary>
	public partial class Planets : Node3D
  { 
    [Export] ResourcePreloader planetTextures;
    /// <summary>
		/// Used to instantiate a new planet.
		/// </summary>
	  [Export] PackedScene PlanetScene { get; set; }

    /// <summary>
		/// Receives the data from <see cref="SkyView"/> to draw the planets.
		/// </summary>
		/// <param name="planets">The list of planets to draw</param>
		/// <param name="planetaryCalculator">The calculator used to determine position/phase.</param>
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