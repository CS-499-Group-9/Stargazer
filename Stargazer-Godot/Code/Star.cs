using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// A star that has been converted from Horizontal Coordinate form, into Godot coordinate form and drawn to the screen.
    /// </summary>
    public partial class Star : Node3D, IHoverable
    {

        private const float radians = (float)Math.PI / 180f;
        private HorizontalStar horizontalStar;
        private IEquatorialCalculator<HorizontalStar> starConverter;
        // Since these are not being connected to anything in the Godot interface, I'm not sure we need to use the Export attribute.
        // They are all accessed/set via code.
        // Not sure what overhead is involved in labeling these as export.

        public int HipparcosId { get { return horizontalStar.HipparcosId ?? 0; } }

        /// <summary>
        /// Rotation from North (X+), in degrees
        /// </summary>
        public float Azimuth { get { return (float)horizontalStar.Azimuth; } }

        /// <summary>
        /// Rotation from Y=0, in degrees.
        /// </summary>
        public float Altitude { get { return (float)horizontalStar.Altitude; } }


        /// <summary>
        /// Distance from (0, 0, 0)
        /// </summary>
        public float Distance { get { return 74f; } }

        /// <summary>
        /// Apparent brightness of the star
        /// </summary>
        public float Magnitude { get { return (float)(horizontalStar.Magnitude); } }

        /// <summary>
        /// Common name of the star
        /// </summary>
        public string StarName { get { return horizontalStar.StarName; } }
        public Vector3 Position3D { get; private set; }
        public Vector2 Position2D;


        private Globals globalVars;
        // Gets the Cartesian position of the Celestial Body
        private Vector3 GetLocation()
        {
            var altRad = Altitude * radians;
            var azRad = Azimuth * radians;
            Vector3 pos = new()
            {
                X = Distance * (Mathf.Cos(azRad) * Mathf.Cos(altRad)),
                Y = Distance * Mathf.Sin(altRad),
                Z = Distance * Mathf.Cos(altRad) * Mathf.Sin(azRad)
            };
            return pos;
        }

        // Called when the node enters the scene tree for the first time.

        /// <summary>
        /// Populates the field according to the field in a <see cref="HorizontalStar"/>
        /// </summary>
        /// <param name="star">The <see cref="HorizontalStar"/></param>
        public override void _Ready()
        {
            globalVars = GetNode<Globals>("/root/Globals");

        }
        public void FromHorizontal(HorizontalStar star, IEquatorialCalculator<HorizontalStar> starConverter)
        {
            horizontalStar = star;
            this.starConverter = starConverter;
            starConverter.UpdatePositionOf(horizontalStar);
            //GD.Print(star.HipparcosId);
            //hipID = (int)star.HipparcosId;
            Position = GetLocation();
            if (Magnitude > 1) Scale = new Vector3(1 / Magnitude, 1 / Magnitude, 1 / Magnitude);
            else Scale = new Vector3(0.6F, 0.6F, 0.6F);
        }


        public override void _Process(double delta)
        {
            starConverter.UpdatePositionOf(horizontalStar);
            Position = GetLocation();
            Position3D = Position;
        }

        /// <inheritdoc/>
        public string GetHoverText()
        {
            return $"{(String.IsNullOrWhiteSpace(StarName) ? "Unnamed Star" : StarName)}\n" +
            $"HIP {HipparcosId}\n" +
            $"Altitude {Altitude}" +
            $"Azimuth {Azimuth}";
        }
    }
}