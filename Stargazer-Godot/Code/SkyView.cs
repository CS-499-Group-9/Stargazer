using DataLayer;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using Godot;
using System;
using System.Collections.Concurrent;

public partial class SkyView : Node3D
{

   
    public override void _Ready()
    {
        var startup = GetParent()?.GetParent().GetParent<Startup>();
        var spawner = GetNode<Spawner>("Stars");
        startup.UserPositionUpdated += spawner.DrawStars;
        var constellationNode = GetNode<Constellations>("Constellations");
        startup.UserPositionUpdated += constellationNode.DrawConstellations;
        startup.ConstellationLinesToggled += constellationNode.ToggleConstellationLines;
        startup.ConstellationNamesToggled += constellationNode.ToggleConstellationLabels;
        base._Ready();
    }
}
