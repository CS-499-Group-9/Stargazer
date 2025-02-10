using Godot;
using System;

public partial class Star : Node3D
{
	[Export] public float azimuth = 0f; // Rotation from North (X+), in degrees.
	[Export] public float altitude = 0f; // Rotation from Y=0, in degrees.
	[Export] public float dist = 45f; // Distance from (0, 0, 0)
	[Export] public float mag = 1f;

	private float radians = (float)Math.PI / 180f;
	
	// Gets the Cartesian position of the Celestial Body
	private Vector3 getLocation(){
        Vector3 pos = new()
        {
            X = dist * Mathf.Cos(altitude * radians) * Mathf.Cos(azimuth * radians),
    		Y = dist * Mathf.Sin(altitude * radians),
    		Z = dist * Mathf.Cos(altitude * radians) * Mathf.Sin(azimuth * radians),
        };
        return pos;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = getLocation();
		if(mag > 1) Scale = new Vector3(1/mag, 1/mag, 1/mag);
		else Scale = new Vector3(0.6F, 0.6F, 0.6F);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
