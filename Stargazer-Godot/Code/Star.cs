using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// A star that has been converted from Horizontal Coordinate form, into Godot coordinate form and drawn to the screen.
    /// </summary>
    public partial class Star : CelestialBody
    {

        private const float radians = (float)Math.PI / 180f;
        private HorizontalStar horizontalStar;
        // Since these are not being connected to anything in the Godot interface, I'm not sure we need to use the Export attribute.
        // They are all accessed/set via code.
        // Not sure what overhead is involved in labeling these as export.

        public int HipparcosId { get { return horizontalStar.HipparcosId ?? 0; } }

        /// <summary>
        /// Apparent brightness of the star
        /// </summary>
        public float Magnitude { get { return (float)(horizontalStar.Magnitude); } }

        /// <summary>
        /// Common name of the star
        /// </summary>
        public string StarName { get { return horizontalStar.StarName; } }
        /// <summary>
        /// Provides asynchronous access to the star's position.
        /// </summary>
        public Vector3 Position3D { get; private set; }
        public Vector2 Position2D;

        public void FromHorizontal(HorizontalStar star, IEquatorialCalculator starConverter) 
        {
            base.FromHorizontal(star, starConverter);
            horizontalStar = star;
            DrawnDistance = 74f;
            // This needs to be changed so that a star's size in the simulation is actually a function of it's magnitude.
            if (Magnitude > 1) Scale = new Vector3(1 / Magnitude, 1 / Magnitude, 1 / Magnitude);
            else Scale = new Vector3(0.6F, 0.6F, 0.6F);

        }


        public override void _Process(double delta)
        {
            base._Process(delta);
            Position3D = Position;
        }

        /// <inheritdoc/>
        public override string GetHoverText()
        {
            return $"{(String.IsNullOrWhiteSpace(StarName) ? "Unnamed Star" : StarName)}\n" +
            $"HIP {HipparcosId}\n" +
            $"Altitude: {Altitude:0.00}°\n" +
            $"Azimuth: {Azimuth:0.00}°\n" + 
            $"Distance: {Distance} light years";
        }

    }
}