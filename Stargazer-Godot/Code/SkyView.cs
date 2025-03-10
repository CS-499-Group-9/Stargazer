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
        startup.RegisterUserUpdateReceiver(spawner.DrawStars);
        var constellationNode = GetNode<Constellations>("Constellations");
        startup.RegisterUserUpdateReceiver(constellationNode.DrawConstellations);
        startup.RegisterConstellationLineToggleReceiver(constellationNode.ToggleConstellationLines);
        startup.RegisterConstellationLabelToggleReceiver(constellationNode.ToggleConstellationLabels);
        base._Ready();
    }
}
