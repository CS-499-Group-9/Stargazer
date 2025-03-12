using Godot;
using System;

namespace Stargazer
{
	/// <summary>
	/// Contains the constellation label.
	/// </summary>
	public partial class LabelNode : Node3D
	{
		/// <summary>
		/// Used to receive the text to display on the label on instantiation.
		/// </summary>
		[Export] public string LabelText { get; set; }


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Get a reference to the label and pass the text.
			var child = GetChild<Label3D>(0);
			child.Text = LabelText;
		}

	}
}