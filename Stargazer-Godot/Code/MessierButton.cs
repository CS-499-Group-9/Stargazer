using Godot;
using System;
using System.Threading.Tasks;

public partial class MessierButton : Control
{
    public Func<double, double, DateTime, Task> NotifyControllerOfUserUpdate;

	// Called when the node enters the scene tree for the first time.


	private async void RunTheThing()
	{
        var coords = new HuntsvilleCoordinates();
        await NotifyControllerOfUserUpdate(coords.latitude, coords.longitude, DateTime.UtcNow);

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
