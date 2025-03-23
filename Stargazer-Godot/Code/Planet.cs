using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;

public partial class Planet : Node3D
{

	private IPlanetaryCalculator<HorizonalPlanet> calculator;
	private HorizonalPlanet horizonalPlanet;
    private float Distance = 74f;
    private const float radians = (float)Math.PI / 180f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		calculator?.UpdatePosition(horizonalPlanet);
        Position = GetLocation();
	}

    private Vector3 GetLocation()
    {
        var altRad = (float)horizonalPlanet.Altitude * radians;
        var azRad = (float)horizonalPlanet.Azimuth * radians;
        Vector3 pos = new()
        {
            X = Distance * (Mathf.Cos(azRad) * Mathf.Cos(altRad)),
            Y = Distance * Mathf.Sin(altRad),
            Z = Distance * Mathf.Cos(altRad) * Mathf.Sin(azRad)
        };
        return pos;
    }

    public void FromHorizontal(HorizonalPlanet horizonalPlanet, IPlanetaryCalculator<HorizonalPlanet> calculator)
    {
        this.horizonalPlanet = horizonalPlanet;
        this.calculator = calculator;
    }
}
