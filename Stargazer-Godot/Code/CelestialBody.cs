using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stargazer
{
    public abstract partial class CelestialBody : Node3D, IHoverable
    {
        private const float radians = (float)Math.PI / 180f;
        private HorizontalBody horizontalBody;
        protected IEquatorialCalculator calculator;

        public abstract string GetHoverText();

        /// <summary>
        /// Rotation from North (X+), in degrees
        /// </summary>
        public float Azimuth { get { return (float)horizontalBody.Azimuth; } }

        /// <summary>
        /// Rotation from Y=0, in degrees.
        /// </summary>
        public float Altitude { get { return (float)horizontalBody.Altitude; } }

        /// <summary>
        /// Distance from (0, 0, 0)
        /// </summary>
        public float Distance { get { return (float)horizontalBody.Distance; } }
        public float DrawnDistance { get; set; }

        public override void _Process(double delta)
        {

            calculator.UpdatePositionOf(horizontalBody);
            Position = GetLocation();
        }

        protected Vector3 GetLocation()
        {
            var dist = DrawnDistance > 70 ? DrawnDistance : 50 + (DrawnDistance / 31) * (70 - 50);
            var altRad = Altitude * radians;
            var azRad = Azimuth * radians;
            Vector3 pos = new()
            {
                X = dist * (Mathf.Cos(azRad) * Mathf.Cos(altRad)),
                Y = dist * Mathf.Sin(altRad),
                Z = dist * Mathf.Cos(altRad) * Mathf.Sin(azRad)
            };
            return pos;
        }

        public void FromHorizontal(HorizontalBody body, IEquatorialCalculator starConverter)
        {
            horizontalBody = body;
            calculator = starConverter;
            starConverter.UpdatePositionOf(horizontalBody);
            Position = GetLocation();
        }
    }
}
