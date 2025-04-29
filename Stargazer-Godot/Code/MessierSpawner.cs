using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Spawns Messier Deep Space Objects on startup.
/// Author: Josh Johner
/// Created: SPR 2025
/// </summary>
public partial class MessierSpawner : Node3D
{
	[Export] PackedScene MessierScene;
	private Node3D messierContainer;

	public override void _Ready()
	{
		messierContainer = new();
		messierContainer.Visible = false;
	}

	/// <summary>
	/// Initially draws all of the objects in the sky. Should be called using the <c>await</c> keyword.
	/// </summary>
	/// <param name="objects">The <see cref="IEnumerable{HorizontalMessierObject}"/>s to draw.</param>
	/// <param name="calculator">The <see cref="IEquatorialCalculator"/> used to calculate each object's position.</param>
	/// <returns></returns>
	public async Task DrawMessierObjects(IEnumerable<HorizontalMessierObject> objects, IEquatorialCalculator calculator)
	{
		var oldContainer = messierContainer;
		messierContainer = new();
		messierContainer.Visible = oldContainer?.Visible ?? false;
        await Task.Run(() =>
        {
            foreach (var messier in objects)
            {
                SpawnMessier(messier, calculator);
            }
        });
		oldContainer?.Free();
		AddChild(messierContainer);
    }

	/// <summary>
	/// Receives the notification to toggle the visibility of Messier Objects
	/// </summary>
	/// <param name="showObjects">True if objects should be visible</param>
	public void ToggleMessierObjects(bool showObjects)
	{
		messierContainer.Visible = showObjects;
	}

    private void SpawnMessier(HorizontalMessierObject horizontalMessier, IEquatorialCalculator calculator)
    {
        MessierObject messierObject = MessierScene.Instantiate<MessierObject>();
        messierObject.FromHorizontal(horizontalMessier, calculator);
        messierContainer.AddChild(messierObject);
    }

	

}
