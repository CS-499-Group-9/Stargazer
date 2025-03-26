using Godot;
using System;

namespace Stargazer
{
	/// <summary>
	/// The graphic object used to draw the lines.
	/// </summary>
	public partial class AzimuthGridlines : MeshInstance3D
	{
		/// <summary>
		/// The radius of the dome.
		/// </summary>
		[Export] public float radius = 75f;
		/// <summary>
		/// The number of vertical (longitudinal) lines.
		/// </summary>
		[Export] public int longitudeSegments = 24;
		/// <summary>
		/// The smoothness of each curve
		/// </summary>
		[Export] public int latitudeSegments = 24;
		/// <summary>
		/// Latitude interval in degrees
		/// </summary>
		[Export] public float latitudeInterval = 15.0f;
		/// <summary>
		/// Calculate cutoff latitude to ensure it aligns with the first visible circle
		/// </summary>
		[Export] public float cutoffLatitude = 75.0f;
		

		private ImmediateMesh mesh;
		private float storedfov;
		private Camera3D camera;
		private StandardMaterial3D greenMaterial;
		/// <summary>
		/// Initially draws the azimuth lines and hides them.
		/// </summary>
		public override void _Ready()
		{
			greenMaterial = new()
			{

				AlbedoColor = new Color(0.4f, 0.8f, 0.4f, 0.8f), // Green color
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
			};
			mesh = new ImmediateMesh();
			DrawLongitudeLines(mesh);
			DrawLatitudeLines(mesh);
			Mesh = mesh;
			Visible = false;

		}

		/// <summary>
		/// Dynamically draws the gridlines based on camera FOV
		/// </summary>
		/// <param name="delta"></param>
        public override void _Process(double delta)
        {
			if (camera == null) return;
            if (camera.Fov < 15 && storedfov >= 15){
				latitudeInterval = 1.0f;
			}
            else if (camera.Fov > 15 && storedfov <= 15){
				latitudeInterval = 10.0f;
			}
            else if (camera.Fov < 35 && storedfov >= 35){
				latitudeInterval = 10.0f;
			}
			else if (camera.Fov > 35 && storedfov <= 35){
				latitudeInterval = 15.0f;
			}
			mesh.ClearSurfaces();
			DrawLongitudeLines(mesh);
			DrawLatitudeLines(mesh);
			storedfov = camera.Fov;
        }

        /// <summary>
        /// The method used receive the <see cref="ControlContainer.AzimuthToggled"/> notification.
        /// </summary>
        /// <param name="showLines">True if the user has requested to show the lines.</param>
        public void ToggleGridlines(bool showLines)
		{
			Visible = showLines;
		}

		/// <summary>
		/// Gets a reference to the <see cref="Camera3D"/> from the <see cref="SkyView"/>
		/// </summary>
		/// <param name="camera"></param>
		public void SetCamera(Camera3D camera)
		{
			this.camera = camera;
			storedfov = camera.Fov;
		}


		private void DrawLongitudeLines(ImmediateMesh imMesh)
		{
			imMesh.SurfaceBegin(Mesh.PrimitiveType.Lines,greenMaterial);

			float cutoffRadians = Mathf.DegToRad(cutoffLatitude);  // Convert cutoff to radians

			for (int i = 0; i < longitudeSegments; i++)
			{
				float lonAngle = i * Mathf.Tau / longitudeSegments;  // Longitude angle (0 to 2π)
				bool isCardinal = i == 0 || i == longitudeSegments / 4 || i == longitudeSegments / 2 || i == 3 * longitudeSegments / 4;

				for (int j = 0; j < latitudeSegments; j++)
				{
					float latAngle1 = -Mathf.Pi / 2 + (j * Mathf.Pi / latitudeSegments);  // Latitude angle (bottom to top)
					float latAngle2 = -Mathf.Pi / 2 + ((j + 1) * Mathf.Pi / latitudeSegments);

					if (latAngle2 > cutoffRadians && !isCardinal)
					{
						latAngle2 = cutoffRadians;
					}

					// If not a cardinal line, stop before cutoff latitude
					if (!isCardinal && Mathf.Abs(latAngle1) > cutoffRadians)
					{
						continue;  // Skip this segment to truncate the line
					}

					Vector3 p1 = new Vector3(
						Mathf.Cos(lonAngle) * Mathf.Cos(latAngle1) * radius,
						Mathf.Sin(latAngle1) * radius,
						Mathf.Sin(lonAngle) * Mathf.Cos(latAngle1) * radius
					);

					Vector3 p2 = new Vector3(
						Mathf.Cos(lonAngle) * Mathf.Cos(latAngle2) * radius,
						Mathf.Sin(latAngle2) * radius,
						Mathf.Sin(lonAngle) * Mathf.Cos(latAngle2) * radius
					);

					imMesh.SurfaceAddVertex(p1);
					imMesh.SurfaceAddVertex(p2);
				}
			}

			imMesh.SurfaceEnd();
		}

		// Function to draw latitude lines
		private void DrawLatitudeLines(ImmediateMesh imMesh)
		{
			imMesh.SurfaceBegin(Mesh.PrimitiveType.Lines,greenMaterial);
			for (int lat = -90; lat <= 90; lat += (int)latitudeInterval)  // Loop through latitudes from -90 to 90 with the interval
			{
				int storelat = lat;
				if (lat > 75){
					lat = 75;
				}
				if (lat < -75){
					lat = -75;
				}
				float latAngle = Mathf.DegToRad(lat);  // Convert to radians

				// Loop through longitude to draw a full circle at this latitude
				for (int lon = 0; lon < longitudeSegments; lon++)
				{
					float lonAngle1 = lon * Mathf.Tau / longitudeSegments;  // Longitude angle (0 to 2π)

						for(int i = 0; i < 4; i++){
						// Convert spherical coordinates to Cartesian (x, y, z)
						Vector3 p1 = new Vector3(
							Mathf.Cos(lonAngle1 + i*(Mathf.Tau / longitudeSegments)/4) * Mathf.Cos(latAngle) * radius,
							Mathf.Sin(latAngle) * radius,
							Mathf.Sin(lonAngle1 + i*(Mathf.Tau / longitudeSegments)/4) * Mathf.Cos(latAngle) * radius
						);

						Vector3 p2 = new Vector3(
							Mathf.Cos(lonAngle1 + (i+1)*(Mathf.Tau / longitudeSegments)/4) * Mathf.Cos(latAngle) * radius,
							Mathf.Sin(latAngle) * radius,
							Mathf.Sin(lonAngle1 + (i+1)*(Mathf.Tau / longitudeSegments)/4) * Mathf.Cos(latAngle) * radius
						);

						imMesh.SurfaceAddVertex(p1);
						imMesh.SurfaceAddVertex(p2);
					}

				}
				lat = storelat;
			}

			imMesh.SurfaceEnd();
		}
	}
}