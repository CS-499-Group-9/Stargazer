using Godot;
using System;

public partial class ConstellationButton : Control
{
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
