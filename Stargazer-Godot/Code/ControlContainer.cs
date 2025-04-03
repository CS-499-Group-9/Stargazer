using DataLayer;
using Godot;
using Stargazer;
using System;
using System.Threading.Tasks;

public partial class ControlContainer : Control
{
    [Export] private LineEdit latField;
    [Export] private LineEdit longField;
    [Export] private LineEdit timeField;
    [Export] private OptionButton AMorPMButton;
    [Export] private Button calendarButton;
    public Action<bool> AzimuthToggled;
    public Action<bool> EquatorialToggled;
	public Action<bool> EquatorLinesToggled;
	public Action<bool> ConstellationsToggled;
	public Action<bool> ConstellationLabelsToggled;
	public Action<bool> MessierObjectsTogggled;
	public Func<double, double, DateTime, Task> UserPositionUpdated;
    public Action RequestScreenshot;



    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("screenshot_key"))
        {
            RequestScreenshot();
        }
    }



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
        string latText = latField.Text;
        string longText = longField.Text;

        // Huntsville Defaults
        double latitude = 34.7304;
        double longitude = -86.5861;

        if (latText != "" || longText != "")
        {
            latitude = double.Parse(latText);
            longitude = double.Parse(longText);
        }
        else
        {
            GD.PrintErr("Invalid latitude/longitude input.");
        }

        string timeText = timeField.Text.Trim();
        string amPmText = AMorPMButton.GetItemText(AMorPMButton.Selected);

        // Splitting time (HH:mm or H:mm)
        string[] timesplit = timeText.Split(':');
        if (timesplit.Length < 2)
        {
            GD.PrintErr("Invalid time format: " + timeText);
            return;
        }

        int hour = Convert.ToInt32(timesplit[0]);
        int minute = Convert.ToInt32(timesplit[1]);

        // Convert to 24 hour
        if (amPmText == "PM" && hour < 12)
        {
            hour += 12;
        }
        else if (amPmText == "AM" && hour == 12)
        {
            hour = 0;
        }

        string timeString = calendarButton.Text.Trim();
        GD.Print(timeString);
        String[] dateVals = timeString.Split('/');
        var parsedDate = DateTime.Parse($"{timeString} {hour:00}:{minute:00}:00");
        GD.Print($"Parsed Time: {parsedDate:yyyy-MM-dd HH:mm:ss}");
        GD.Print($"Latitude: {latitude}, Longitude: {longitude}");
        
        await UserPositionUpdated(latitude, longitude, parsedDate);
    }
}
