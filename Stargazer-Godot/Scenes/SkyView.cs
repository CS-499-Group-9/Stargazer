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
        var globalVars = GetNode<Globals>("/root/Globals"); // Import globals
        var controller = globalVars.Startup;
        var spawner = GetNode<Spawner>("Stars");
        controller.UserPositionUpdated += spawner.DrawStars;
        var constellationNode = GetNode<Constellations>("Constellations");
        base._Ready();
    }
}
