using DataLayer;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


		private Node3D StarContainer;
		private IEquatorialConverter<HorizontalStar> starConverter;
		const float maxStarMagnitude = 6.0f;
		/// <summary>
		/// Receives the notification to update the stars drawn.
		/// </summary>
		/// <param name="stars">The <see cref="IEnumerable{HorizontalStar}"/> that contains the stars to draw.</param>
		public async Task DrawStars(IEnumerable<HorizontalStar> stars, Func<int, Func<HorizontalStar, Star>, Star> GetStar, IEquatorialConverter<HorizontalStar> starConverter)
		{
			this.starConverter = starConverter;
			// Get a reference to the current star container
			var oldContainer = StarContainer;

			// Create a new star container in memory
			StarContainer = new();
			
			// Create a new task to calculate the positions of the stars and add them to the container and await completion
			await Task.Run(() =>
			{
				foreach (var star in stars.Where((s) =>  s.Magnitude < maxStarMagnitude))
				{
					GetStar(star?.HipparcosId ?? 0, SpawnStar);
				}
			});

			// If the previous container exists, remove it from the tree then add the new container.
			oldContainer?.Free();
			AddChild(StarContainer);
		}

		/// <summary>
		/// Used to instantiate a new star and place it in the scene tree (draw it).
		/// </summary>
		/// <param name="horizontalStar">The <see cref="HorizontalStar"/> to base the <see cref="Star"/> on.</param>
		/// <returns>A reference to the new <see cref="Star"/></returns>
		public Star SpawnStar(HorizontalStar horizontalStar)
		{

			Star star = StarScene.Instantiate<Star>();
			star.FromHorizontal(horizontalStar, starConverter);
			// Because this may be called by the constellation container after the star container has been added to the tree...
			if (StarContainer.IsInsideTree())
			{
				// Can only be added by the main thread. Notify Godot to complete this at the end of the current frame.
				StarContainer.CallDeferred("add_child", star);
			}
			else
			{
				// Use whatever thread has called this method.
				StarContainer.AddChild(star);
			}
			return star;
		}
	}
}
