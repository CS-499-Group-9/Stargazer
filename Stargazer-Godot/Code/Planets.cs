using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System.Collections.Generic;

namespace Stargazer
{
	/// <summary>
	/// A <see cref="Node3D"/> container for the planets.
	/// Author: Josh Johner
	/// Created: SPR 2025
	/// Refactored by William Arnett (SPR 2025) to use custom shader.
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
		public void DrawPlanets(IEnumerable<HorizontalPlanet> planets, IEquatorialCalculator planetaryCalculator)
		{
			foreach (var child in GetChildren()) { child.Free(); }

			foreach (var planet in planets)
			{
				var newPlanet = PlanetScene.Instantiate<Planet>();
				newPlanet.FromHorizontal(planet, planetaryCalculator);
				newPlanet.SetTexture(GD.Load<Texture2D>($"res://Textures/Resources/{planet.Name.ToLower()}map.tres"));
				AddChild(newPlanet);
			}
		}
	}
}