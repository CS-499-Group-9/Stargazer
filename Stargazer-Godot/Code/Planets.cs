using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System.Collections.Generic;

namespace Stargazer
{
    /// <summary>
    /// A <see cref="Node3D"/> container for the planets.
    /// </summary>
    public partial class Planets : Node3D
    {
        /// <summary>
        /// Used to instantiate a new planet.
        /// </summary>
        [Export] PackedScene PlanetScene { get; set; }
        // Called when the node enters the scene tree for the first time.

        /// <summary>
        /// Receives the data from <see cref="SkyView"/> to draw the planets.
        /// </summary>
        /// <param name="planets">The list of planets to draw</param>
        /// <param name="planetaryCalculator">The calculator used to determine position/phase.</param>
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