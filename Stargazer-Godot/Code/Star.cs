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

		[Export] public float azimuth = 0f; // Rotation from North (X+), in degrees.
		[Export] public float altitude = 0f; // Rotation from Y=0, in degrees.
		[Export] public float dist = 74f; // Distance from (0, 0, 0)
		[Export] public float mag = 1f;
		[Export] public string starName;

		// Gets the Cartesian position of the Celestial Body
		private Vector3 getLocation()
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
		public override void _Ready()
		{
			Position = getLocation();
			if (mag > 1) Scale = new Vector3(1 / mag, 1 / mag, 1 / mag);
			else Scale = new Vector3(0.6F, 0.6F, 0.6F);

		}

	}
}