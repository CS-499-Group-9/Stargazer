using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MessierSpawner : Node3D
{
	[Export] PackedScene MessierScene;
	private Node3D messierContainer;
	private IEquatorialCalculator calculator;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async Task DrawMessierObjects(IEnumerable<HorizontalMessierObject> objects, IEquatorialCalculator calculator)
	{
		this.calculator = calculator;
		var oldContainer = messierContainer;
		messierContainer = new();
        await Task.Run(() =>
        {
            foreach (var messier in objects)
            {
                SpawnMessier(messier);
            }
        });
		oldContainer?.Free();
		AddChild(messierContainer);
    }

    private void SpawnMessier(HorizontalMessierObject horizontalMessier)
    {
        MessierObject messierObject = MessierScene.Instantiate<MessierObject>();
        messierObject.FromHorizontal(horizontalMessier, calculator);
        messierContainer.AddChild(messierObject);
    }

}
