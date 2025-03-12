using Godot;
using System;

namespace Stargazer
{
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

		/// <summary>
		/// Gets a reference to the <see cref="Camera3D"/> in the <see cref="SkyView"/>
		/// </summary>
		public override void _Ready()
		{
			Node viewNode = GetTree()?.Root.FindChild("View", true, false);
			camera = (Camera3D)viewNode?.GetNode("Camera3D");
		}

		/// <summary>
		/// Rotates the compass needle as the user view is rotated.
		/// </summary>
		/// <param name="delta"></param>
		public override void _Process(double delta)
		{
			float yRotationDegrees = Mathf.RadToDeg(camera.Rotation.Y);
			needle.RotationDegrees = yRotationDegrees + 90; // Camera starts West for some reason.
		}
	}
}