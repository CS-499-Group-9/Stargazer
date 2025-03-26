using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// A moon object to be drawn.
    /// </summary>
    public partial class Moon : Node3D, IHoverable
    {
        private HorizontalMoon horizontalMoon;
        private IMoonCalculator calculator;
        private float Distance = 74f;
        private const float radians = (float)Math.PI / 180f;

        /// <summary>
        /// Initializes object data when it enters the tree.
        /// </summary>
        public override void _Ready()
        {
            Scale = new Vector3(4, 4, 4);
        }

        /// <summary>
        /// Calculates phase and positional data every frame.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            calculator?.UpdatePositionOf(horizontalMoon);
            Position = GetLocation();
            LookAt(Vector3.Up);
        }

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
        /// Passes in data necessary to perform calculations.
        /// </summary>
        /// <param name="moon">The object to base the calculations on</param>
        /// <param name="moonCalculator">The calculator used to perform the calculations.</param>
        public void FromHorizontal(HorizontalMoon moon, IMoonCalculator moonCalculator)
        {
            horizontalMoon = moon;
            calculator = moonCalculator;
        }

        /// <inheritdoc/>
        public string GetHoverText()
        {
            return $"The Moon\n" +
            $"Altitude {horizontalMoon.Altitude}\n" +
            $"Azimuth {horizontalMoon.Azimuth}";
        }
    }
}