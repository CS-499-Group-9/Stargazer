using Godot;
using System;

public partial class AzimuthGridlines : MeshInstance3D
{
	[Export] public float radius = 49.5f;
	[Export] public int longitudeSegments = 24;  // Number of vertical (longitude) lines
	[Export] public int latitudeSegments = 24;   // Smoothness of each curve
	[Export] public float latitudeInterval = 15.0f;  // Latitude interval in degrees
	[Export] public float cutoffLatitude = 75.0f;  // Calculate cutoff latitude to ensure it aligns with the first visible circle

	private Globals globalVars;
	private ImmediateMesh mesh;

	public override void _Ready()
	{
		mesh = new ImmediateMesh();
		globalVars = GetNode<Globals>("/root/Globals");
	}


    public override void _Process(double delta)
    {
    	if(globalVars.isAzimuth){
			DrawLongitudeLines(mesh);
			DrawLatitudeLines(mesh);
			this.Mesh = mesh;
		}
		else{
            mesh.ClearSurfaces();
			this.Mesh = mesh;
        }
    }
    
    // Function to draw longitude lines
    private void DrawLongitudeLines(ImmediateMesh imMesh)
	{
		imMesh.ClearSurfaces();
		imMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

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
		imMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

		for (int lat = -90; lat <= 90; lat += (int)latitudeInterval)  // Loop through latitudes from -90 to 90 with the interval
		{
			float latAngle = Mathf.DegToRad(lat);  // Convert to radians

			// Loop through longitude to draw a full circle at this latitude
			for (int lon = 0; lon < longitudeSegments; lon++)
			{
				float lonAngle1 = lon * Mathf.Tau / longitudeSegments;  // Longitude angle (0 to 2π)

				// Convert spherical coordinates to Cartesian (x, y, z)
				Vector3 p1 = new Vector3(
					Mathf.Cos(lonAngle1) * Mathf.Cos(latAngle) * radius,
					Mathf.Sin(latAngle) * radius,
					Mathf.Sin(lonAngle1) * Mathf.Cos(latAngle) * radius
				);

				Vector3 p2 = new Vector3(
					Mathf.Cos(lonAngle1 + (Mathf.Tau / longitudeSegments)) * Mathf.Cos(latAngle) * radius,
					Mathf.Sin(latAngle) * radius,
					Mathf.Sin(lonAngle1 + (Mathf.Tau / longitudeSegments)) * Mathf.Cos(latAngle) * radius
				);

				imMesh.SurfaceAddVertex(p1);
				imMesh.SurfaceAddVertex(p2);
			}
		}

		imMesh.SurfaceEnd();
	}
}
