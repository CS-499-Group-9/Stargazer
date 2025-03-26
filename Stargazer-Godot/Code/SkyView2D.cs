using DataLayer;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;
using System.Threading.Tasks;

namespace Stargazer
{
	/// <summary>
	/// The 2D viewport used for screenshot functionality.
	/// </summary>
	public partial class SkyView2D : Node2D, IUserUpdateReceiver
	{
		private Spawner2D Stars;
		private Constellations2D Constellations;


		/// <summary>
		/// Gathers references to it's children.
		/// </summary>
		public override void _Ready()
		{
			Stars = GetNode<Spawner2D>("Stars2D");
			Constellations = GetNode<Constellations2D>("Constellations2D");
		}

		/// <summary>
		/// Receives the data from <see cref="SkyView"/> used to draw the scene
		/// </summary>
		/// <param name="dataPackage">The entire data package. This is not needed. It will be replaced by only what is needed.</param>
		/// <returns>A task that can be awaited.</returns>
		public async Task UpdateUserPosition(CelestialDataPackage<Star> dataPackage)
		{
			await Stars.DrawStars(dataPackage.DrawnStars);
			await Constellations.DrawConstellations(dataPackage.Constellations, dataPackage.GetStar);
		}
	}
}