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
	/// The <see cref="Node3D"/> containing all of the <see cref="Planet"/>s
	/// </summary>
	public partial class Planets : Node3D
	{
		/// <summary>
		/// The <see cref="PackedScene"/> used to instantiate new planets.
		/// </summary>
		[Export] PackedScene PlanetScene { get; set; }

		/// <summary>
		/// Used to notify the container to draw a new set of planets
		/// </summary>
		/// <param name="planets">The list of planets to draw.</param>
		/// <param name="planetaryCalculator">The calculator to pass to each of the planets so that it can locate itself.</param>
		public void DrawPlanets(IEnumerable<HorizontalPlanet> planets, IPlanetaryCalculator<HorizontalPlanet> planetaryCalculator)
		{
			foreach (var child in GetChildren()) { child.Free(); }

			foreach (var planet in planets)
			{
				var newPlanet = PlanetScene.Instantiate<Planet>();
				newPlanet.FromHorizontal(planet, planetaryCalculator);
				AddChild(newPlanet);
			}
		}
	}
}