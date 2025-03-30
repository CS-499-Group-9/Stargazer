using Godot;
using Stargazer;
using System;
using System.Threading.Tasks;

public partial class ControlContainer : Control
{
	public Action<bool> AzimuthToggled;
    public Action<bool> EquatorialToggled;
	public Action<bool> EquatorLinesToggled;
	public Action<bool> ConstellationsToggled;
	public Action<bool> ConstellationLabelsToggled;
	public Action<bool> MessierObjectsTogggled;
	public Func<double, double, DateTime, Task> UserPositionUpdated;

    /// <summary>
    /// Receives the <see cref="Signal"/> from the <see cref="AzimuthButton"/>'s <see cref="CheckBox"/> and broadcasts on the <see cref="AzimuthToggled"/> notification.
    /// </summary>
    /// <param name="value"></param>
	public void ToggleAzimuth(bool value) { AzimuthToggled?.Invoke(value); }

    public void ToggleEquatorial(bool value) {EquatorialToggled?.Invoke(value); }

    /// Receives the <see cref="Signal"/> from the <see cref="ConstellationButton"/>'s <see cref="CheckBox"/> and broadcasts on the <see cref="ConstellationsToggled"/> notification.
    public void ToggleConstellations(bool value) { ConstellationsToggled?.Invoke(value); }

    /// Receives the <see cref="Signal"/> from the <see cref="LabelButton"/>'s <see cref="CheckBox"/> and broadcasts on the <see cref="ConstellationLabelsToggled"/> notification.
    public void ToggleConstellationLabels(bool value) { ConstellationLabelsToggled?.Invoke(value); }

    /// Receives the <see cref="Signal"/> from the <see cref="MessierButton"/>'s <see cref="CheckBox"/> and broadcasts on the <see cref="MessierObjectsTogggled"/> notification.

	public void ToggleMessierObjects(bool value) { MessierObjectsTogggled?.Invoke(value); }

    /// <summary>
    /// Dummy method to broadcast a hardcoded user request. Will be replaced. 
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
