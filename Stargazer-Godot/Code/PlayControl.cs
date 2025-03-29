using Godot;
using System;

public partial class PlayControl : Control
{
    public Func<int, int> PlaySpeedUpdated;
    public Action SyncronizeTime;
    private int multiplier = 1;
    private Label multiplierLabel;

    private const int secsPerMin = 60;
    private const int minPerHour = 60;
    private const int secsPerHour = secsPerMin * minPerHour;
    private const int hourPerDay = 24;
    private const int secsPerDay = secsPerHour * hourPerDay;
    private const int daysPerYear = 365;
    private const int secsPerYear = secsPerDay * daysPerYear;
    private const int yearsPerCentury = 100;

    public void DecreaseTen() { UpdateMultiplierLable(PlaySpeedUpdated(-secsPerDay)); }
    public void DecreaseOne() { UpdateMultiplierLable(PlaySpeedUpdated(-secsPerMin)); }
    public void OnSyncronizeTime() { SyncronizeTime(); UpdateMultiplierLable(1); }
    public void PlayNormal() { UpdateMultiplierLable(PlaySpeedUpdated(0)); }
    public void IncreaseOne() { UpdateMultiplierLable(PlaySpeedUpdated(secsPerMin)); }
    public void IncreaseTen() { UpdateMultiplierLable(PlaySpeedUpdated(secsPerDay)); }

    public override void _Ready()
    {
        base._Ready();

        multiplierLabel = GetNode<Label>("MarginContainer/VBoxContainer/MultiplierLabel");
    }

    private void UpdateMultiplierLable(int multiplier)
    {
        int years = multiplier / secsPerYear;
        multiplier %= secsPerYear;
        int days = multiplier / secsPerDay;
        multiplier %= secsPerDay;
        int hours = multiplier / secsPerHour;
        multiplier %= secsPerHour;
        int minutes = multiplier / secsPerMin;
        multiplier %= secsPerMin;
        int seconds = multiplier;

        var yearString = years > 0 ? $"{years} years" : null;
        var dayString = days > 0 ? $"{days} days" : null;
        var hourString = hours > 0 ? $"{hours} hours" : null;
        var minuteString = minutes > 0 ? $"{minutes} minutes" : null;
        var secondsString = seconds > 0 ? $"{seconds} second" : null;

        var outputString = string.Join(',', yearString, dayString, hourString, minuteString, secondsString);
        multiplierLabel.Text = outputString ?? "Paused";

    }

}
