using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;

public partial class MessierObject : CelestialBody
{

	private HorizontalMessierObject horizontalMessierObject;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Scale = new Vector3(2,2,2);
		return;
		var Magnitude = (float)horizontalMessierObject.Magnitude;
        if (Magnitude > 1) Scale = new Vector3(1 / Magnitude, 1 / Magnitude, 1 / Magnitude);
        else Scale = new Vector3(0.6F, 0.6F, 0.6F);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override string GetHoverText()
    {
		return $"{horizontalMessierObject.MessierId}\n + " +
			$"Altitde: {horizontalMessierObject.Altitude}\n" + 
			$"Azimuth: {horizontalMessierObject.Azimuth}\n + " +
			$"Distance: {horizontalMessierObject.Distance}\n" + 
			$"Size: {horizontalMessierObject.Size} lightyears\n" + 
			$"Viewing Season: {horizontalMessierObject.ViewingSeason}";
    }

	public void FromHorizontal(HorizontalMessierObject messierObject, IEquatorialCalculator calculator)
	{
		horizontalMessierObject = messierObject;
		base.FromHorizontal(messierObject, calculator);
	}
}
