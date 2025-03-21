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

		public float futureAltitude = 0f;

		public float futureAzimuth = 0f;
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
		public int hipID;
		public Vector2 Pos2D;
		private Globals globalVars;

        public override void _Ready()
        {
            globalVars = GetNode<Globals>("/root/Globals");
        }
        public override void _Process(double delta)
        {
            Position = GetLocation(LerpCoords());
        }

		// Gets the Cartesian position of the Celestial Body
		private Vector3 GetLocation(Vector2 altazi)
		{
			var altRad = altazi[0] * radians;
			var azRad = altazi[1] * radians;
			Vector3 pos = new()
			{
				X = dist * (Mathf.Cos(azRad) * Mathf.Cos(altRad)),
				Y = dist * Mathf.Sin(altRad),
				Z = dist * Mathf.Cos(altRad) * Mathf.Sin(azRad)
			};
			return pos;
		}

        // 
        /// <summary>
        /// Populates the field according to the field in a <see cref="HorizontalStar"/>
		/// Called when the node enters the scene tree for the first time.
        /// </summary>
        /// <param name="star">The <see cref="HorizontalStar"/></param>
        public void FromHorizontal(HorizontalStar star)
		{
			azimuth = (float)star.Azimuth;
			altitude = (float)star.Altitude;
			futureAltitude = (float)star.FutureAltitude;
			futureAzimuth = (float)star.FutureAzimuth;
			mag = (float)star.Magnitude;
			starName = star.StarName;
			
			//GD.Print(star.HipparcosId);
			if(star.HipparcosId != null){
				hipID = (int)star.HipparcosId;
			}else{
				hipID = 0;
			}
			//hipID = (int)star.HipparcosId;
			Position = GetLocation(new Vector2(altitude,azimuth));
            if (mag > 1) Scale = new Vector3(1 / mag, 1 / mag, 1 / mag);
            else Scale = new Vector3(0.6F, 0.6F, 0.6F);
        }
		private Vector2 LerpCoords(){
			//Rolling over from 1 degree to 359
			var calcAzimuth = azimuth;
			var calcFutureAzimuth = futureAzimuth;
			calcAzimuth = (futureAzimuth - calcAzimuth)%360;
			if(futureAzimuth - azimuth > 300){
				calcAzimuth += 360;
			}
			//Rolling over from 359 degrees to 1
			else if(futureAzimuth - azimuth < -300){
				calcFutureAzimuth += 360;
			}
			return new Vector2(
				x:(float)(altitude + (futureAltitude-altitude)*((DateTime.UtcNow - globalVars.requestTime).TotalSeconds/120)),
				y:(float)(azimuth + (calcFutureAzimuth-calcAzimuth)*((DateTime.UtcNow - globalVars.requestTime).TotalSeconds/120))
			);
		}

    }
}