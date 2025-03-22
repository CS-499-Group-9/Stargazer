using DataLayer;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stargazer
{

	/// <summary>
	/// Draws all stars that are not a part of constellations
	/// </summary>
	public partial class Spawner : Node3D
	{
		/// <summary>
		/// The scene used to instantiate the star objects.
		/// </summary>
		[Export] public PackedScene StarScene { get; set; }
		/// <summary>
		/// The scene used to instantiate the labels for stars
		/// </summary>
		[Export] public PackedScene LabelScene { get; set; }

		private Node3D StarContainer;

		/// <summary>
		/// Receives the notification to update the stars drawn.
		/// </summary>
		/// <param name="stars">The <see cref="IEnumerable{HorizontalStar}"/> that contains the stars to draw.</param>
		public async Task DrawStars(IEnumerable<HorizontalStar> stars)
		{
			// Get a reference to the current star container
			var oldContainer = StarContainer;

			// Create a new star container in memory
			StarContainer = new();
			
			// Create a new task to calcualte the positions of the stars and add them to the container and await completion
			await Task.Run(() =>
			{
				foreach (var star in stars)
				{
					SpawnStar(star);
				}
			});

			// If the previous container exists, remove it from the tree then add the new container.
			oldContainer?.Free();
			AddChild(StarContainer);
		}

		// For the record, I very much dislike repeating this block of code in Constellations.cs, but I haven't figured out how to offload that just yet. 
		// Perhaps more to follow.....
		// TODO: Consider moving this up to the parent node. This would have to be handled elegantly so that it can be reused by Spawner, Constellations and MessierObjects
		private Star SpawnStar(HorizontalStar horizontalStar)
		{
// 			GD.Print($@"
// Star ID: {horizontalStar.StarId}
//  Hip ID: {horizontalStar.HipparcosId}");
			Star star = StarScene.Instantiate<Star>();
			star.FromHorizontal(horizontalStar);
			StarContainer.AddChild(star);
			return star;
		}
	}
}
