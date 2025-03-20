using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using Godot;

namespace Stargazer
{
	

	/// <summary>
	/// Draws all stars that are not a part of constellations
	/// </summary>
	public partial class Spawner2D : Node2D
	{
		private int count = 0;
		private const float radians = (float)Math.PI / 180f;
		/// <summary>
		/// The scene used to instantiate the star objects.
		/// </summary>
		[Export] public PackedScene Star2DScene { get; set; }
		/// <summary>
		/// The scene used to instantiate the labels for stars
		/// </summary>
		[Export] public PackedScene Label2DScene { get; set; }

		private Node2D Star2dContainer;
		/// <summary>
		/// Receives the notification to update the stars drawn.
		/// </summary>
		/// <param name="dataPackage">The <see cref="CelestialDataPackage{Star}"/> that contains the stars to draw.</param>
		public async Task DrawStars(IEnumerable<HorizontalStar> stars)
		{
			var oldContainer = Star2dContainer;
			Star2dContainer = new();

			await Task.Run(() =>
			{
				foreach (var star in stars)
				{
					SpawnStar(star);
				}
			});	
			oldContainer?.Free();
			
			AddChild(Star2dContainer);

		}

		// For the record, I very much dislike repeating this block of code in Constellations.cs, but I haven't figured out how to offload that just yet. 
		// Perhaps more to follow.....
		// TODO: Consider moving this up to the parent node. This would have to be handled elegantly so that it can be reused by Spawner, Constellations and MessierObjects
		private Star2D SpawnStar(HorizontalStar horizontalStar)
		{
			Star2D outstar = Star2DScene.Instantiate<Star2D>();
			outstar.azimuth = (float)horizontalStar.Azimuth;
			outstar.altitude = (float)horizontalStar.Altitude;
			outstar.mag = (float)horizontalStar.Magnitude;
			outstar.starName = horizontalStar.StarName;
			Star2dContainer.AddChild(outstar);
			return outstar;
		}


    }
}