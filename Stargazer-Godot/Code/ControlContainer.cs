using DataLayer;
using Godot;
using Stargazer;
using System;
using System.Threading.Tasks;

public partial class ControlContainer : Control
{
    [Export] private LineEdit latDegField;
    [Export] private LineEdit latMinField;
    [Export] private LineEdit lonDegField;
    [Export] private LineEdit lonMinField;
    [Export] private LineEdit timeField;
    [Export] private OptionButton AMorPMButton;
    [Export] private Button calendarButton;

    /// <summary>
    /// Notifies the subscribers when the user has toggled the azimuth grid
    /// </summary>
    public Action<bool> AzimuthToggled;
    /// <summary>
    /// Notifies the subscribers when the user has toggled the equatorial grid
    /// </summary>
    public Action<bool> EquatorialToggled;
    /// <summary>
    /// Notifies the subscribers when the user has toggle the visibility of constellations
    /// </summary>
	public Action<bool> ConstellationsToggled;
    /// <summary>
    /// Notifies the subscribers when the user has toggled the visibility of constellation labels
    /// </summary>
	public Action<bool> ConstellationLabelsToggled;
    /// <summary>
    /// Notifies the subscribers when the user has toggled the visibility of Messier objects
    /// </summary>
	public Action<bool> MessierObjectsToggled;
    /// <summary>
    /// Notifies the subscriber (one only) of a request to update to the observers position/time.
    /// </summary>
	public Action<double, double, DateTime> UserPositionUpdated;
    /// <summary>
    /// Broadcasts a request to take a screenshot
    /// </summary>
    public Func<Task> RequestScreenshot;

    /// <summary>
    /// Checks to see if the user has requested a screenshot
    /// </summary>
    /// <param name="delta">Unused</param>
    public async override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("screenshot_key"))
        {
            await RequestScreenshot();
        }
    }

    /// <summary>
    /// Receives the <see cref="Signal"/> from the AzimuthButton's <see cref="CheckBox"/> and broadcasts on the <see cref="AzimuthToggled"/> notification.
    /// </summary>
    /// <param name="value">True if the toggle button is checked</param>
    public void ToggleAzimuth(bool value) { AzimuthToggled?.Invoke(value); }
    /// <summary>
    /// Receives the <see cref="Signal"/> from the <see cref=""/>
    /// </summary>
    /// <param name="value">True if the toggle button is checked.</param>
    public void ToggleEquatorial(bool value) {EquatorialToggled?.Invoke(value); }

    /// <summary>
    /// Receives the <see cref="Signal"/> from the ConstellationButton's <see cref="CheckBox"/> and broadcasts on the <see cref="ConstellationsToggled"/> notification.
    /// </summary>
    /// <param name="value">True if the toggle button is checked</param>
    public void ToggleConstellations(bool value) { ConstellationsToggled?.Invoke(value); }

    /// <summary>
    /// Receives the <see cref="Signal"/> from the LabelButton's <see cref="CheckBox"/> and broadcasts on the <see cref="ConstellationLabelsToggled"/> notification.
    /// </summary>
    /// <param name="value">True if the checkbox is checked</param>
    public void ToggleConstellationLabels(bool value) { ConstellationLabelsToggled?.Invoke(value); }


    /// <summary>
    /// Receives the <see cref="Signal"/> from the "MessierButton's <see cref="CheckBox"/> and broadcasts on the <see cref="MessierObjectsToggled"/> notification.
    /// </summary>
    /// <param name="value">True if the checkbox is checked</param>
    public void ToggleMessierObjects(bool value) { MessierObjectsToggled?.Invoke(value); }

    /// <summary>
    /// Receives the <c>Enter</c> button's <see cref="Signal"/>, gathers user input and broadcasts <see cref="UserPositionUpdated"/>
    /// </summary>
	public async void UpdateUserPosition()
    {
        string latDegText = latDegField.Text;
        string latMinText = latMinField.Text;
        string lonDegText = lonDegField.Text;
        string lonMinText = lonMinField.Text;

        // Default to 0s
        double latitude = 0;
        double longitude = 0;

        if (latDegText != "" || lonDegText != "")
        {
            latitude = int.Parse(latDegText);
            longitude = int.Parse(lonDegText);
        }
        else if (latMinText != "" || lonMinText != "")
        {
            latitude += double.Parse(latMinText) * 0.016667;
            longitude += double.Parse(lonMinText) * 0.016667;
        }
        else
        {
            GD.PrintErr("Invalid latitude/longitude input.");
            return;
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
        
        UserPositionUpdated(latitude, longitude, parsedDate.ToUniversalTime());
    }
}
