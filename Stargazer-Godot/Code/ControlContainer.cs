using Godot;
using Stargazer;
using System;
using System.Threading.Tasks;

namespace Stargazer
{
	/// <summary>
	/// A vertical container that holds all of the user interface controls.
	/// </summary>
	public partial class ControlContainer : VBoxContainer
	{
		/// <summary>
		/// Used to notify subscribers of the <see cref="AzimuthButton.ToggleAzimuth(bool)"/> notification.
		/// </summary>
		public Action<bool> AzimuthToggled;
		/// <summary>
		/// Used to notify subscribers of the <see cref="ConstellationButton.ToggleConst(bool)"/> notification.
		/// </summary>
		public Action<bool> ConstellationsToggled;
		/// <summary>
		/// Used to notify the subscribers of the <see cref="LabelButton.ToggleLabel(bool)"/> notification.
		/// </summary>
		public Action<bool> ConstellationLabelsToggled;
		/// <summary>
		/// Used to notify the subscribers of the <see cref="MessierButton.ToggleMessierObjects(bool)"/>
		/// </summary>
		public Action<bool> MessierObjectsTogggled;
		/// <summary>
		/// Used to notify the subscribers 
		/// </summary>
		public Func<double, double, DateTime, Task> UserPositionUpdated;

		/// <summary>
		/// Temporary method used to make the request to <see cref="Startup.UpdateUserPosition(double, double, DateTime)"/>
		/// This will be replaced inside of the eventual parent of <see cref="TimeLocEntered"/>
		/// </summary>
		public async void UpdateUserPosition()
		{
			Globals globalVars = GetNode<Globals>("/root/Globals"); // Import globals
			var coords = new HuntsvilleCoordinates();
			globalVars.requestTime = DateTime.UtcNow;
			GD.Print("Update");
			await UserPositionUpdated(coords.latitude, coords.longitude, DateTime.UtcNow);
		}

		private struct HuntsvilleCoordinates
		{
			public double latitude = 34.7304;
			public double longitude = -86.5861;
			public HuntsvilleCoordinates()
			{
			}
		}
	}
}