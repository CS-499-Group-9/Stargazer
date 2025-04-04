using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;

public partial class Sun : CelestialBody
{
	private HorizontalSun star;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Scale = new Vector3(2,2,2);
	}

    public override void _Process(double delta)
    {
        calculator?.UpdatePositionOf(star);
        Position = GetLocation();
        Rotate(Vector3.Up, Mathf.Pi);
		RotationDegrees = new Vector3(0, 0, -90 + 34.7304f);

    }


    public override string GetHoverText()
    {
		return $"The Sun\n" +
			$"Altitude: {star.Altitude}\n" +
			$"Azimuth: {star.Azimuth}\n"+
			$"Distance: {star.Distance}";
    }

	public void FromHorizontal(HorizontalSun star, IEquatorialCalculator calculator)
	{
		base.FromHorizontal(star, calculator);
		this.star = star;
		this.calculator = calculator;
	}
}
