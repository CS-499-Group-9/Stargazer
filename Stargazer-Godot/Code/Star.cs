using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Stargazer
{
	/// <summary>
	/// A star that has been converted from Horizontal Coordinate form, into Godot coordinate form and drawn to the screen.
	/// </summary>
	public partial class Star : Node3D
	{
        private Globals globalVars;
        private const float radians = (float)Math.PI / 180f;
		private HorizontalStar horizontalStar;
		private IEquatorialConverter<HorizontalStar> starConverter;
		
		/// <summary>
		/// The Hipparcos Id of the star
		/// </summary>
		public int HipparcosId { get { return horizontalStar.HipparcosId ?? 0; } }

		/// <summary>
		/// Rotation from North (X+), in degrees
		/// </summary>
		public float Azimuth { get{ return (float)horizontalStar.Azimuth; } }

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

		/// <summary>
		/// Used to safely request the position of the star from a thread other than the main thread.
		/// </summary>
		public Vector3 Position3D { get; private set; }

		/// <summary>
		/// Will be used to get the 2D position of the star (maybe... maybe not..)
		/// </summary>
		public Vector2 Position2D;


		/// <summary>
		/// Calculates the position of the object in Godot cartesian coordinates.
		/// </summary>
		/// <returns>A <see cref="Vector3"/> for the position.</returns>
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

        /// <inheritdoc/>
        public override void _Ready()
        {
            globalVars = GetNode<Globals>("/root/Globals");
			
        }

		/// <summary>
		/// Calculates the position of the star each frame.
		/// </summary>
		/// <param name="delta">Unused.</param>
        public override void _Process(double delta)
        {
			starConverter.UpdatePositionOf(horizontalStar);
            Position = GetLocation();
			Position3D = Position;
        }

        /// <summary>
        /// Populates the field according to the field in a <see cref="HorizontalStar"/>
        /// </summary>
        /// <param name="star">The <see cref="HorizontalStar"/></param>
        public void FromHorizontal(HorizontalStar star, IEquatorialConverter<HorizontalStar> starConverter)
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


    }
}