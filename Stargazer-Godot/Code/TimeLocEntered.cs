using Godot;
using System;
namespace Stargazer
{
	/// <summary>
	/// Used to gather user input and notify the <see cref="Startup"/> object of a new user request.
	/// </summary>
	public partial class TimeLocEntered : Control
	{
		private Node latField;
		private Node longField;
		private LineEdit timeField;
		private OptionButton AMorPMButton;
		private Node calendarButton;


		public override void _Ready()
		{
			latField = GetTree().Root.FindChild("LatitudeButton", true, false);
			longField = GetTree().Root.FindChild("LongitudeButton", true, false);
			timeField = GetTree().Root.FindChild("TimeLineEdit", true, false) as LineEdit;
			AMorPMButton = GetTree().Root.FindChild("TimeHalfSelector", true, false) as OptionButton;
		}

		private void OnDateSelected(object dateObj)
		{
			// Assuming dateObj has a method Date that returns a formatted date string
			GD.Print(dateObj.ToString());
		}

		private void ButtonPressed()
		{
			string latText = latField.GetChild<LineEdit>(1).Text;
			string longText = longField.GetChild<LineEdit>(1).Text;

			// Huntsville Defaults
			double latitude = 34.7304;
			double longitude = -86.5861;

			if (latText == "" || longText == "")
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

			DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);
			GD.Print($"Parsed Time: {time:yyyy-MM-dd HH:mm:ss}");
			GD.Print($"Latitude: {latitude}, Longitude: {longitude}");
		}
	}
}