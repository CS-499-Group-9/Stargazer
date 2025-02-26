using Godot;
using System;

public partial class Globals : Node
{
    public static Globals Instance {get; private set; }
    public bool isAzimuth {get; set; }
    [Signal] public delegate void ScreenshotRequestedEventHandler();
    
    public override void _Ready(){
        Instance = this;
        isAzimuth = false; // Default to Azimuth view
    }
}
