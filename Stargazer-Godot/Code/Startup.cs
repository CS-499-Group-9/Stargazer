using DataLayer;
using Godot;
using NewGameProject.Code;
using System;
using System.Threading.Tasks;

public partial class Startup : Control
{
    private const string viewPortPath = "SubViewportContainer/SubViewport/View";
    private StargazerRepositoryService<Star> repositoryService;

    public event EventHandler<CelestialDataPackage<Star>> UserPositionUpdated;
    public event EventHandler<bool> ConstellationLinesToggled;
    public event EventHandler<bool> ConstellationNamesToggled;
    public event EventHandler<bool> AzimuthLinesToggled;


    public async override void _Ready()
    {
        repositoryService = await InjectionService<Star>.GetRepositoryServiceAsync(ProjectSettings.GlobalizePath("res://"));

    }



    public async Task UpdateUserPosition(double latitude, double longitude, DateTime dateTime)
    { 
        GD.Print("Updating user position");
        var dataPackage = await repositoryService.UpdateUserPosition(latitude, longitude, dateTime);
        UserPositionUpdated?.Invoke(this, dataPackage);

    }

    public void ToggleConstellationLines(object sender, bool showLines)
    {
        ConstellationLinesToggled?.Invoke(this, showLines);
    }

    public void ToggleConstellationNames(object sender, bool showNames)
    {
        ConstellationNamesToggled?.Invoke(this, showNames);
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
