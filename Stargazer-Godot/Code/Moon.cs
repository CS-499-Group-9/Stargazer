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
        Transform3D rotateTransform = new Transform3D();
        Vector3 forward = (-Position.Normalized());  
        Vector3 up = new Vector3(Mathf.Cos(Mathf.DegToRad(34.7304f)),Mathf.Sin(Mathf.DegToRad(34.7304f)),0f);
        Vector3 left = forward.Cross(up).Normalized();
        Basis rotateBasis = new Basis(left,up,-forward);
        rotateTransform.Origin = Position;
        rotateTransform.Basis = rotateBasis;
        Transform = rotateTransform;
        Scale = new Vector3(4, 4, 4);
        //RotateZ(-90+34.7304f);
        // RotationDegrees = new Vector3(0,0,-90+34.7304f);
        //LookAt(Vector3.Up);
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

    public string GetHoverText()
    {
                    return $"The Moon\n"+
                    $"Altitude {horizontalMoon.Altitude}\n"+
                    $"Azimuth {horizontalMoon.Azimuth}";
    }

    public Transform3D getGlobalTransform()
    {
        return GlobalTransform;
    }
}
