using Godot;
using System;

public partial class ConstellationButton : Control
{
	public Action<bool> ConstellationLinesToggled;
	private bool isVisible;

	public override void _Ready()
	{
		var startup = GetParent<Startup>();
	}



	private void ToggleConst(bool state)
	{
		ConstellationLinesToggled(state);
	}
}
