using DataLayer.Interfaces;
using Godot;
using System.Collections;

namespace Stargazer
{
	/// <summary>
	/// The graphic object used to draw the lines.
	/// </summary>
	public partial class AzimuthGridlines : Node3D
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
		[Export] public float longitudeInterval = 15.0f;
		/// <summary>
		/// Calculate cutoff latitude to ensure it aligns with the first visible circle
		/// </summary>
		[Export] public float cutoffLatitude = 75.0f;
		
		public MeshInstance3D azimuthGridlines;
		public MeshInstance3D equatorialGridlines;
		private ImmediateMesh mesh,mesh2;
		private StandardMaterial3D orangeMaterial;
		private StandardMaterial3D blueMaterial;
		private IEquatorialCalculator calculator;
		private Globals globalVars;
        float latitude = 90+34.7304f;
        float longitude = -86.5861f;
		//const float theta = 30;
		/// <summary>
		/// Initially draws the azimuth lines and hides them.
		/// </summary>
		public override void _Ready()
		{
        	globalVars = GetNode<Globals>("/root/Globals"); // Import globals
			orangeMaterial = new()
			{

				AlbedoColor = new Color(0.8f, 0.5f, 0.4f, 0.8f), // Green color
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
			};
			blueMaterial = new(){
				AlbedoColor = new Color(0.2f, 0.4f, 1.0f, 0.8f), // Green color
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
			};
			mesh = new ImmediateMesh();
			mesh2 = new ImmediateMesh();
			DrawLongitudeLines(mesh,mesh2);
			DrawLatitudeLines(mesh,mesh2);
			
			azimuthGridlines = new MeshInstance3D();
			equatorialGridlines = new MeshInstance3D();
			azimuthGridlines.Mesh = mesh;
			equatorialGridlines.Mesh = mesh2;
			equatorialGridlines.Visible = true;
			AddChild(azimuthGridlines);
			AddChild(equatorialGridlines);
			azimuthGridlines.Visible = false;
			equatorialGridlines.Visible = false;

		}

        /// <summary>
        /// Dynamically draws the gridlines based on camera FOV
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
			if (calculator != null)
			{
				latitude = (float)calculator.Latitude + 90;
				longitude = (float)calculator.Longitude;
			}
			mesh.ClearSurfaces();
			mesh2.ClearSurfaces();
			DrawLongitudeLines(mesh,mesh2);
			DrawLatitudeLines(mesh,mesh2);

        }

		public void HandleZoomStateChanged(ZoomState zoomState)
		{
			switch (zoomState)
			{
				case ZoomState.FullOut:
					latitudeInterval = longitudeInterval = 15.0f;
					break;
				case ZoomState.Middle:
					latitudeInterval = longitudeInterval = 10.0f;
					break;
				case ZoomState.FullIn:
					latitudeInterval = longitudeInterval = 1.0f;
					break;
			}
		}
        /// <summary>
        /// The method used receive the <see cref="ControlContainer.AzimuthToggled"/> notification.
        /// </summary>
        /// <param name="showLines">True if the user has requested to show the lines.</param>
        public void ToggleGridlines(bool showLines)
		{
			azimuthGridlines.Visible = showLines;
		}
        public void ToggleEquatorialGridlines(bool showLines)
		{
			equatorialGridlines.Visible = showLines;
		}

		public void SetCalculator(IEquatorialCalculator calculator)
		{
			this.calculator = calculator;
		}


		// Function to draw longitude lines
		private void DrawLongitudeLines(ImmediateMesh imMesh, ImmediateMesh imMesh2)

		{
			float theta = -(float)((calculator?.LST ?? 0)*15+longitude);
			imMesh.SurfaceBegin(Mesh.PrimitiveType.Lines,orangeMaterial);
			imMesh2.SurfaceBegin(Mesh.PrimitiveType.Lines,blueMaterial);
			float cutoffRadians = Mathf.DegToRad(cutoffLatitude);  // Convert cutoff to radians
			float sinlat = Mathf.Sin(latitude*Mathf.Pi/180f);
			float coslat = Mathf.Cos(latitude*Mathf.Pi/180f);
			float sintheta = Mathf.Sin(theta*Mathf.Pi/180f);
			float costheta = Mathf.Cos(theta*Mathf.Pi/180f);
			for (int i = 0; i < 360; i += (int)longitudeInterval){
				//float lonAngle = i * Mathf.Tau / longitudeSegments;  // Longitude angle (0 to 2π)
				float lonAngle = i*Mathf.Pi/180f;
				//bool isCardinal = i == 0 || i == longitudeSegments / 4 || i == longitudeSegments / 2 || i == 3 * longitudeSegments / 4;

                for (int j = 0; j < latitudeSegments; j++)
                {
                    float latAngle1 = -Mathf.Pi / 2 + (j * Mathf.Pi / latitudeSegments);  // Latitude angle (bottom to top)
                    float latAngle2 = -Mathf.Pi / 2 + ((j + 1) * Mathf.Pi / latitudeSegments);

					if (latAngle2 > cutoffRadians)
					{
						latAngle2 = cutoffRadians;
					}

					// If not a cardinal line, stop before cutoff latitude
					if (Mathf.Abs(latAngle1) > cutoffRadians)
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

					Vector3 p1new = new Vector3(
						p1.X*costheta - p1.Z*sintheta,
						p1.Y,
						p1.X*sintheta + p1.Z*costheta
					);

					Vector3 p2new = new Vector3(
						p2.X*costheta - p2.Z*sintheta,
						p2.Y,
						p2.X*sintheta + p2.Z*costheta
					);

					Vector3 p1newer = new Vector3(
						p1new.X*coslat - p1new.Y*sinlat,
						p1new.X*sinlat + p1new.Y*coslat,
						p1new.Z
					);
					Vector3 p2newer = new Vector3(
						p2new.X*coslat - p2new.Y*sinlat,
						p2new.X*sinlat + p2new.Y*coslat,
						p2new.Z
					);
					imMesh.SurfaceAddVertex(p1);
					imMesh.SurfaceAddVertex(p2);
					imMesh2.SurfaceAddVertex(0.9f*p1newer);
					imMesh2.SurfaceAddVertex(0.9f*p2newer);
				}
			}

			imMesh.SurfaceEnd();
			imMesh2.SurfaceEnd();
		}

		// Function to draw latitude lines
		private void DrawLatitudeLines(ImmediateMesh imMesh,ImmediateMesh imMesh2)
		{
			float theta = -(float)(calculator?.LST ?? 0)*15+longitude;
			float sinlat = Mathf.Sin(latitude*Mathf.Pi/180f);
			float coslat = Mathf.Cos(latitude*Mathf.Pi/180f);
			float sintheta = Mathf.Sin(theta*Mathf.Pi/180f);
			float costheta = Mathf.Cos(theta*Mathf.Pi/180f);
			imMesh.SurfaceBegin(Mesh.PrimitiveType.Lines,orangeMaterial);
			imMesh2.SurfaceBegin(Mesh.PrimitiveType.Lines,blueMaterial);
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

                    for (int i = 0; i < 4; i++)
                    {
                        // Convert spherical coordinates to Cartesian (x, y, z)
                        Vector3 p1 = new Vector3(
                            Mathf.Cos(lonAngle1 + i * (Mathf.Tau / longitudeSegments) / 4) * Mathf.Cos(latAngle) * radius,
                            Mathf.Sin(latAngle) * radius,
                            Mathf.Sin(lonAngle1 + i * (Mathf.Tau / longitudeSegments) / 4) * Mathf.Cos(latAngle) * radius
                        );

						Vector3 p2 = new Vector3(
							Mathf.Cos(lonAngle1 + (i+1)*(Mathf.Tau / longitudeSegments)/4) * Mathf.Cos(latAngle) * radius,
							Mathf.Sin(latAngle) * radius,
							Mathf.Sin(lonAngle1 + (i+1)*(Mathf.Tau / longitudeSegments)/4) * Mathf.Cos(latAngle) * radius
						);
						Vector3 p1new = new Vector3(
							p1.X*costheta - p1.Z*sintheta,
							p1.Y,
							p1.X*sintheta + p1.Z*costheta
						);

						Vector3 p2new = new Vector3(
							p2.X*costheta - p2.Z*sintheta,
							p2.Y,
							p2.X*sintheta + p2.Z*costheta
						);

						Vector3 p1newer = new Vector3(
							p1new.X*coslat - p1new.Y*sinlat,
							p1new.X*sinlat + p1new.Y*coslat,
							p1new.Z
						);
						Vector3 p2newer = new Vector3(
							p2new.X*coslat - p2new.Y*sinlat,
							p2new.X*sinlat + p2new.Y*coslat,
							p2new.Z
						);
						imMesh.SurfaceAddVertex(p1);
						imMesh.SurfaceAddVertex(p2);
						imMesh2.SurfaceAddVertex(0.9f*p1newer);
						imMesh2.SurfaceAddVertex(0.9f*p2newer);
					}

                }
                lat = storelat;
            }

			imMesh.SurfaceEnd();
			imMesh2.SurfaceEnd();
		}
	}
}