using Godot;
using System;

public partial class PlayControl : Control
{
	public Action<float> PlaySpeedUpdated;
	public Action SyncronizeTime;

	public void DecreaseTen() { PlaySpeedUpdated(-500); }
	public void DecreaseOne() { PlaySpeedUpdated(-50); }
	public void OnSyncronizeTime() { SyncronizeTime(); }
	public void PlayNormal() { PlaySpeedUpdated(0); }
	public void IncreaseOne() { PlaySpeedUpdated(50); }
	public void IncreaseTen() { PlaySpeedUpdated(500); }

   

    
}
