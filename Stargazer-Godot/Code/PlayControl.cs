using Godot;
using Stargazer;
using System;

public partial class PlayControl : Control
{
    private Label multiplierLabel;
    private PlaySpeed multiplier;
    

    public void UpdateSeconds(int seconds) 
    {
        multiplier.IncreaseBySeconds(seconds);
        UpdateMultiplierLabel(); 
    }
    public void UpdateMinutes(int minutes) 
    {
        multiplier.IncreaseByMinutes(minutes);
        UpdateMultiplierLabel(); 
    }

    public void UpdateHours(int hours)
    {
        multiplier.IncreateByHours(hours);
        UpdateMultiplierLabel();
    }

    public void UpdateDays(int days) 
    {
        multiplier.IncreaseByDays(days);
        UpdateMultiplierLabel() ;
    }

    public void OnSynchronizeTime() 
    {
        multiplier.SynchronizeTime();
        UpdateMultiplierLabel(); 
    }

    public void PlayNormal() 
    { 
        multiplier.RealTime();
        UpdateMultiplierLabel();
    }

    public override void _Ready()
    {
        base._Ready();
        multiplier = new PlaySpeed();
        multiplierLabel = GetNode<Label>("MarginContainer/VBoxContainer/MultiplierLabel");
        UpdateMultiplierLabel();
    }

    public PlaySpeed Activate()
    {
        Visible = true;
        UpdateMultiplierLabel();
        return multiplier;
    }

    private void UpdateMultiplierLabel()
    {
       multiplierLabel.Text = $"{multiplier.ToString()} per second";
    }

}
