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
		/// Used to notify subscribers of the <see cref="ToggleAzimuth(bool)"/> notification.
		/// </summary>
		public Action<bool> AzimuthToggled;
		/// <summary>
		/// Used to notify subscribers of the <see cref="ToggleConstellations(bool)"/> notification.
		/// </summary>
		public Action<bool> ConstellationsToggled;
		/// <summary>
		/// Used to notify the subscribers of the <see cref="ToggleConstellationLabels(bool)"/> notification.
		/// </summary>
		public Action<bool> ConstellationLabelsToggled;
		/// <summary>
		/// Used to notify the subscribers of the <see cref="ToggleMessierObjects(bool)"/> notification.
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

		/// <summary>
		/// Receives the <see cref="AzimuthButton"/> <see cref="CheckBox"/> On Toggle <see cref="Signal"/> and notifies subscribers of the <see cref="AzimuthToggled"/> notification.
		/// </summary>
		/// <param name="value">True if the gridlines are to be displayed.</param>
        public void ToggleAzimuth(bool value) { AzimuthToggled?.Invoke(value); }

		/// <summary>
		/// Receives the <see cref="ConstellationButton"/> <see cref="CheckBox"/> On Toggle <see cref="Signal"/> and notifies the subscribers of the <see cref="ConstellationsToggled"/> notification.
		/// </summary>
		/// <param name="value">True if the constellations are to be displayed.</param>
        public void ToggleConstellations(bool value) { ConstellationsToggled?.Invoke(value); }

		/// <summary>
		/// Receives the <see cref="LabelButton"/> <see cref="CheckBox"/> On Toggle <see cref="Signal"/> and notifies the subscribers of the <see cref="ConstellationLabelsToggled"/> notification.
		/// </summary>
		/// <param name="value">True if the constellation labels are to be displayed.</param>
        public void ToggleConstellationLabels(bool value) { ConstellationLabelsToggled?.Invoke(value); }

		/// <summary>
		/// Receives the <see cref="MessierButton"/> <see cref="CheckBox"/> On Toggle <see cref="Signal"/> and notifies the subscribers of the <see cref="MessierObjectsTogggled"/> notification.
		/// </summary>
		/// <param name="value">True if the Messier Objects are to be displayed.</param>
        public void ToggleMessierObjects(bool value) { MessierObjectsTogggled?.Invoke(value); }

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