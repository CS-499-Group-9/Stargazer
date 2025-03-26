using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// The <see cref="Node3D"/> used to represent a planet.
    /// </summary>
    public partial class Planet : Node3D
    {

        private IPlanetaryCalculator<HorizontalPlanet> calculator;
        private HorizontalPlanet horizonalPlanet;
        private float Distance = 74f;
        private const float radians = (float)Math.PI / 180f;


        /// <summary>
        /// Calculates the position and phase of the planet each frame.
        /// </summary>
        /// <param name="delta">The number of seconds (or fraction of) since the last frame.</param>
        public override void _Process(double delta)
        {
            calculator?.UpdatePositionOf(horizonalPlanet);
            Position = GetLocation();
        }

        /// <summary>
        /// Used to calculate the current Godot coordinates of the planet. 
        /// </summary>
        /// <returns>A <see cref="Vector3"/> for the current location.</returns>
        private Vector3 GetLocation()
        {
            var altRad = (float)horizonalPlanet.Altitude * radians;
            var azRad = (float)horizonalPlanet.Azimuth * radians;
            Vector3 pos = new()
            {
                X = Distance * (Mathf.Cos(azRad) * Mathf.Cos(altRad)),
                Y = Distance * Mathf.Sin(altRad),
                Z = Distance * Mathf.Cos(altRad) * Mathf.Sin(azRad)
            };
            return pos;
        }

        /// <summary>
        /// Used to pass the information the planet needs to locate itself.
        /// </summary>
        /// <param name="horizontalPlanet">The <see cref="HorizontalPlanet"/> the calculations are based on.</param>
        /// <param name="calculator">The <see cref="IPlanetaryCalculator{HorizontalPlanet}"/> used to perform the calculations.</param>
        public void FromHorizontal(HorizontalPlanet horizontalPlanet, IPlanetaryCalculator<HorizontalPlanet> calculator)
        {
            this.horizonalPlanet = horizontalPlanet;
            this.calculator = calculator;
        }
    }
}