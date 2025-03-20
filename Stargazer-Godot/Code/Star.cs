using DataLayer.HorizontalObjects;
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

		private const float radians = (float)Math.PI / 180f;

		// Since these are not being connected to anything in the Godot interface, I'm not sure we need to use the Export attribute.
		// They are all accessed/set via code.
		// Not sure what overhead is involved in labeling these as export.

		/// <summary>
		/// Rotation from North (X+), in degrees
		/// </summary>
		public float azimuth = 0f;

        /// <summary>
        /// Rotation from Y=0, in degrees.
        /// </summary>
        public float altitude = 0f;

        /// <summary>
        /// Distance from (0, 0, 0)
        /// </summary>
        public float dist = 74f;

		/// <summary>
		/// Apparent brightness of the star
		/// </summary>
		public float mag = 1f;

		/// <summary>
		/// Common name of the star
		/// </summary>
		public string starName;

		// Gets the Cartesian position of the Celestial Body
		private Vector3 GetLocation()
		{
			var altRad = altitude * radians;
			var azRad = azimuth * radians;
			Vector3 pos = new()
			{
				X = dist * (Mathf.Cos(azRad) * Mathf.Cos(altRad)),
				Y = dist * Mathf.Sin(altRad),
				Z = dist * Mathf.Cos(altRad) * Mathf.Sin(azRad)
			};
			return pos;
		}
		// Called when the node enters the scene tree for the first time.
		
		/// <summary>
		/// Populates the field according to the field in a <see cref="HorizontalStar"/>
		/// </summary>
		/// <param name="star">The <see cref="HorizontalStar"/></param>
        public void FromHorizontal(HorizontalStar star)
		{
			azimuth = (float)star.Azimuth;
			altitude = (float)star.Altitude;
			mag = (float)star.Magnitude;
			starName = star.StarName;
			Position = GetLocation();
            if (mag > 1) Scale = new Vector3(1 / mag, 1 / mag, 1 / mag);
            else Scale = new Vector3(0.6F, 0.6F, 0.6F);
        }

    }
}