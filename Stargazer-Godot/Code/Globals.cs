using Godot;
using System;

public partial class Globals : Node
{
    public static Globals Instance {get; private set; }
    public bool isAzimuth {get; set; }
    public bool isConstellation {get; set; }
    public bool isLabel {get; set; }
    public override void _Ready(){
        Instance = this;
        isAzimuth = false; // Default for Azimuth view
        isConstellation = true; // Default for Constellations
        isLabel = true; // Default for Labels
    }
}
