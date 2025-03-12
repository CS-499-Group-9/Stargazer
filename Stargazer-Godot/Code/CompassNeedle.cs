using Godot;
using System;

/// <summary>
/// Displays a compass needle to orient the user relative to North.
/// </summary>
public partial class CompassNeedle : Sprite2D
{
	/// <summary>
	/// The needle to display
	/// </summary>
	[Export] public Node2D needle;
	private Camera3D camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Node viewNode = GetTree()?.Root.FindChild("View", true, false);
		camera = (Camera3D)viewNode?.GetNode("Camera3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float yRotationDegrees = Mathf.RadToDeg(camera.Rotation.Y);
		needle.RotationDegrees = yRotationDegrees + 90; // Camera starts West for some reason.
	}
}
