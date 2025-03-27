using Godot;
using Stargazer;
using System;
using System.Threading.Tasks;

public partial class ControlContainer : VBoxContainer
{
	public Action<bool> AzimuthToggled;
	public Action<bool> EquatorLinesToggled;
	public Action<bool> ConstellationsToggled;
	public Action<bool> ConstellationLabelsToggled;
	public Action<bool> MessierObjectsTogggled;
	public Func<double, double, DateTime, Task> UserPositionUpdated;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ToggleAzimuth(bool value) { AzimuthToggled?.Invoke(value); }
	public void ToggleEquator(bool value) { EquatorLinesToggled?.Invoke(value); }
	public void ToggleConstellations(bool value) { ConstellationsToggled?.Invoke(value); }
	public void ToggleConstellationLabels(bool value) { ConstellationLabelsToggled?.Invoke(value); }
	public void ToggleMessierObjects(bool value) { MessierObjectsTogggled?.Invoke(value); }

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
