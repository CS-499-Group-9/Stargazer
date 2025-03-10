using Godot;
using System;

public class coordData
{
    public double latitude;
    public double longitude;
    public DateTime timestamp;
}

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }
    public bool isAzimuth { get; set; }
    public bool isConstellation { get; set; }
    public bool isLabel { get; set; }

    public coordData data = new coordData(); // Create a global instance

    public override void _Ready()
    {
        Instance = this;
        isAzimuth = false;
        isConstellation = true;
        isLabel = true;
        
        // Huntsville by default
        data.timestamp = DateTime.Now.ToUniversalTime();
        data.latitude = 34.7304;
        data.longitude = -86.5861;
    }
}