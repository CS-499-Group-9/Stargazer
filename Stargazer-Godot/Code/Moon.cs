using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// The <see cref="Node3D"/> representing the moon.
    /// </summary>
    public partial class Moon : Node3D
    {
        private HorizontalMoon horizontalMoon;
        private IMoonCalculator calculator;
        private float Distance = 74f;
        private const float radians = (float)Math.PI / 180f;


        /// <summary>
        /// Called when the moon enters the scene tree.
        /// Used to set the scale to a constant value.
        /// </summary>
        public override void _Ready()
        {
            Scale = new Vector3(10, 10, 10);
        }

        /// <summary>
        /// Recalculates the position and phase of the moon each frame
        /// </summary>
        /// <param name="delta">The number of seconds (or fraction of) since the last frame.</param>
        public override void _Process(double delta)
        {
            calculator?.UpdatePositionOf(horizontalMoon);
            Position = GetLocation();
        }

        /// <summary>
        /// Calculates the position of the moon in Godot coordinates
        /// </summary>
        /// <returns>A <see cref="Vector3"/> of the moon's position.</returns>
        private Vector3 GetLocation()
        {
            var altRad = (float)horizontalMoon.Altitude * radians;
            var azRad = (float)horizontalMoon.Azimuth * radians;
            Vector3 pos = new()
            {
                X = Distance * (Mathf.Cos(azRad) * Mathf.Cos(altRad)),
                Y = Distance * Mathf.Sin(altRad),
                Z = Distance * Mathf.Cos(altRad) * Mathf.Sin(azRad)
            };
            return pos;
        }

        /// <summary>
        /// Used to set the internal object used for calculations
        /// </summary>
        /// <param name="moon">The <see cref="HorizontalMoon"/> object to perform calculations on.</param>
        /// <param name="moonCalculator">The <see cref="IMoonCalculator"/> used to perform the calculations.</param>
        public void FromHorizontal(HorizontalMoon moon, IMoonCalculator moonCalculator)
        {
            horizontalMoon = moon;
            calculator = moonCalculator;
        }
    }
}