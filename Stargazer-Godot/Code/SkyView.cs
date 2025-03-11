using DataLayer;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using Godot;
using System;
using System.Collections.Concurrent;

public partial class SkyView : Node3D
{
    public Action<CelestialDataPackage<Star>> UpdateUserPosition;
    public Action<bool> ToggleConstellationLines;
    public Action<bool> ToggleConstellationLabels;
    public Action<bool> ToggleGridlines;
   
    public override void _Ready()
    {
        base._Ready();
        var startup = GetParent()?.GetParent().GetParent<Startup>();
        var spawner = GetNode<Spawner>("Stars");
        UpdateUserPosition += spawner.DrawStars;
        var constellationNode = GetNode<Constellations>("Constellations");
        UpdateUserPosition += constellationNode.DrawConstellations;
        ToggleConstellationLines = constellationNode.ToggleConstellationLines;
        ToggleConstellationLabels = constellationNode.ToggleConstellationLabels;
        var azimuthGridlines = GetNode<AzimuthGridlines>("Dome/Azimuth Gridlines");
        ToggleGridlines = azimuthGridlines.ToggleGridlines;
    }
}
