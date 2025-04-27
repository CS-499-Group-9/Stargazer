using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// A moon object to be drawn.
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// Refactored by William Arnett (SPR 2025) to handle rotation and get hover text.
    /// </summary>
    public partial class Moon : CelestialBody
    {
        private HorizontalMoon horizontalMoon;
    

        /// <summary>
        /// Calculates phase and positional data every frame.
        /// Must override the <see cref="CelestialBody._Process(double)"/> method since calculations are moon specific.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            calculator?.UpdatePositionOf(horizontalMoon);
            DrawnDistance = Distance;
            Position = GetLocation();

            // Rotate the object to always face the earth.
            Transform3D rotateTransform = new Transform3D();
            Vector3 forward = (-Position.Normalized());  
            Vector3 up = new Vector3(Mathf.Cos(Mathf.DegToRad(34.7304f)),Mathf.Sin(Mathf.DegToRad(34.7304f)),0f);
            Vector3 left = forward.Cross(up).Normalized();
            Basis rotateBasis = new Basis(left,up,-forward);
            rotateTransform.Origin = Position;
            rotateTransform.Basis = rotateBasis;
            Transform = rotateTransform;
            Scale = new Vector3(2,2,2);
        }

        /// <inheritdoc/>
        public override string GetHoverText()
        {
            return $"The Moon\n" +
            $"Altitude {horizontalMoon.Altitude}\n" +
            $"Azimuth {horizontalMoon.Azimuth}\n" +
            $"Distance: {horizontalMoon.Distance} AU";
        }

        /// <summary>
        /// Passes in data necessary to perform calculations.
        /// </summary>
        /// <param name="moon">The object to base the calculations on</param>
        /// <param name="moonCalculator">The calculator used to perform the calculations.</param>
        public void FromHorizontal(HorizontalMoon moon, IEquatorialCalculator moonCalculator)
        {
            base.FromHorizontal(moon, moonCalculator);
            horizontalMoon = moon;
            DrawnDistance = Distance;
            calculator = moonCalculator;
        }

    }
}

