using Godot;
using System;

public partial class ConstellationButton : Control
{
	public event EventHandler<bool> Clicked;
	private bool isVisible;

	private Globals globalVars;
	public override void _Ready()
	{
		globalVars = GetNode<Globals>("/root/Globals");
	}



	private void ToggleConst(bool state)
	{
		globalVars.isConstellation = state;
	}
}
