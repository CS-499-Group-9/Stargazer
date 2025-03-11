using DataLayer;
using Godot;
using System;
using System.Threading.Tasks;

public partial class Startup : Control
{
    private const string viewPortPath = "SubViewportContainer/SubViewport/View";
    private StargazerRepositoryService<Star> repositoryService;

    public event Action<CelestialDataPackage<Star>> UserPositionUpdated;

    public async override void _Ready()
    {
        repositoryService = await InjectionService<Star>.GetRepositoryServiceAsync(ProjectSettings.GlobalizePath("res://"));
        
        var skyView = GetNode<SkyView>("SubViewportContainer/SubViewport/View");
        UserPositionUpdated = skyView.UpdateUserPosition;

        var constellationButton = GetNode<ConstellationButton>("ConstellationButton");
        constellationButton.ConstellationLinesToggled = skyView.ToggleConstellationLines;

        var labelButton = GetNode<LabelButton>("LabelButton");
        labelButton.ConstellationLabelsToggled = skyView.ToggleConstellationLabels;

        var azimuthButton = GetNode<AzimuthButton>("AzimuthButton");
        azimuthButton.GridlinesToggled = skyView.ToggleGridlines;


    }

   
    public async Task UpdateUserPosition(double latitude, double longitude, DateTime dateTime)
    { 
        GD.Print("Updating user position");
        var dataPackage = await repositoryService.UpdateUserPosition(latitude, longitude, dateTime);
        UserPositionUpdated?.Invoke(dataPackage);
    }

   
}
