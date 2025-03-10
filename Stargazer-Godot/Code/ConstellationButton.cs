using Godot;
using System;

public partial class ConstellationButton : Control
{
	public event EventHandler<bool> Clicked;
	private bool isVisible;

	public override void _Ready()
	{
		var startup = GetParent<Startup>();
		Clicked += startup.ToggleConstellationLines;
	}



	private void ToggleConst(bool state)
	{
		Clicked?.Invoke(this, state);
	}
}
