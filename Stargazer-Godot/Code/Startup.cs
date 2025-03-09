using DataLayer;
using Godot;
using System;
using System.Threading.Tasks;

public partial class Startup : Control
{
    private const string viewPortPath = "SubViewportContainer/SubViewport/View";
    private StargazerRepositoryService<Star> repositoryService;

    public event EventHandler<CelestialDataPackage<Star>> UserPositionUpdated;
    public event EventHandler<bool> ToggleConstellationLines;
    public event EventHandler<bool> ToggleConstellationNames;
    public event EventHandler<bool> ToggleAzimuthLines;

    public async override void _Ready()
    {
        repositoryService = await InjectionService<Star>.GetRepositoryServiceAsync(ProjectSettings.GlobalizePath("res://"));
        var globalVars = GetNode<Globals>("/root/Globals"); // Import globals
        globalVars.Startup = this;
        var coords = new HuntsvilleCoordinates();
        await UpdateUserPosition(coords.latitude, coords.longitude, DateTime.UtcNow);
    }


    public async Task UpdateUserPosition(double latitude, double longitude, DateTime userLocalTime)
    {
        var dataPackage = await repositoryService.UpdateUserPosition(latitude, longitude, userLocalTime);
        UserPositionUpdated?.Invoke(this, dataPackage);
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
