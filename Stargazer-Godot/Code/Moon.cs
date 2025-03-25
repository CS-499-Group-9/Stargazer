using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;

public partial class Moon : Node3D,IHoverable
{
    private HorizontalMoon horizontalMoon;
    private IMoonCalculator calculator;
    private float Distance = 74f;
    private const float radians = (float)Math.PI / 180f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Scale = new Vector3(4, 4, 4);
	}
    public override void _Process(double delta)
    {
        calculator?.UpdatePosition(horizontalMoon);
        Position = GetLocation();
        LookAt(Vector3.Up);
    }

    private Vector3 GetLocation()
    {
        var altRad = (float)horizontalMoon.Altitude * radians;
        var azRad = (float)horizontalMoon.Azimuth * radians;
        Vector3 pos = new()
        {
            X = Distance * (Mathf.Cos(azRad) * Mathf.Cos(altRad)),
            Y = Distance * Mathf.Sin(altRad),
            Z = Distance * Mathf.Cos(altRad) * Mathf.Sin(azRad)
        };
        return pos;
    }

    public void FromHorizontal(HorizontalMoon moon, IMoonCalculator moonCalculator)
	{
		horizontalMoon = moon;
		calculator = moonCalculator;
	}

    public string getHoverText()
    {
                    return $"The Moon\n"+
                    $"Altitude {horizontalMoon.Altitude}\n"+
                    $"Azimuth {horizontalMoon.Azimuth}";
    }
}
