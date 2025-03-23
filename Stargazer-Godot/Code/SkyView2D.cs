using DataLayer;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;
using System.Threading.Tasks;

public partial class SkyView2D : Node2D, IUserUpdateReceiver
{
	private Spawner2D Stars;
	private Constellations2D Constellations;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Stars = GetNode<Spawner2D>("Stars2D");
		Constellations = GetNode<Constellations2D>("Constellations2D");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

    public async Task UpdateUserPosition(CelestialDataPackage<Star> dataPackage)
    {
        await Stars.DrawStars(dataPackage.DrawnStars);
		await Constellations.DrawConstellations(dataPackage.Constellations, dataPackage.GetStar);
    }
}
