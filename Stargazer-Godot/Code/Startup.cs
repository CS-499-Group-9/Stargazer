using DataLayer;
using Godot;
using NewGameProject.Code;
using System;
using System.Threading.Tasks;

public partial class Startup : Control
{
    private const string viewPortPath = "SubViewportContainer/SubViewport/View";
    private StargazerRepositoryService<Star> repositoryService;

    public event Action<CelestialDataPackage<Star>> UserPositionUpdated;
    public event Action<bool> ConstellationLinesToggled;
    public event Action<bool> ConstellationNamesToggled;
    public event Action<bool> AzimuthLinesToggled;


    public async override void _Ready()
    {
        repositoryService = await InjectionService<Star>.GetRepositoryServiceAsync(ProjectSettings.GlobalizePath("res://"));

    }

    public void RegisterUserUpdateReceiver(Action<CelestialDataPackage<Star>> UpdateMethod)
    {
        UserPositionUpdated += UpdateMethod;
    }

    public void RegisterConstellationLineToggleReceiver(Action<bool> Receiver) 
    {
        ConstellationLinesToggled = Receiver;
    }

    public void RegisterConstellationLabelToggleReceiver(Action<bool> Receiver)
    {
        ConstellationNamesToggled = Receiver;
    }

    public async Task UpdateUserPosition(double latitude, double longitude, DateTime dateTime)
    { 
        GD.Print("Updating user position");
        var dataPackage = await repositoryService.UpdateUserPosition(latitude, longitude, dateTime);
        UserPositionUpdated?.Invoke(dataPackage);
    }

    public void ToggleConstellationLines(object sender, bool showLines)
    {
        ConstellationLinesToggled(showLines);
    }

    public void ToggleConstellationNames(object sender, bool showNames)
    {
        ConstellationNamesToggled(showNames);
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
