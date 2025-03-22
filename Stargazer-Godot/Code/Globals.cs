using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// Currently is no longer being used due to <see cref="Action"/>s and <see cref="Delegate"/>s
    /// </summary>
    public partial class Globals : Node
    {
        /// <summary>
        /// The singleton instance
        /// </summary>
    public static Globals Instance { get; private set; }
    public bool isAzimuth { get; set; }
    public bool isConstellation { get; set; }
    public bool isLabel { get; set; }
    public bool isHover { get; set; }
    public string hoverLabel { get; set; }

    public coordData data = new coordData(); // Create a global instance

    public override void _Ready()
    {
        Instance = this;
        isAzimuth = false;
        isConstellation = true;
        isLabel = true;
        isHover = false;
        hoverLabel = "";
        
        // Huntsville by default
        data.timestamp = DateTime.Now.ToUniversalTime();
        data.latitude = 34.7304;
        data.longitude = -86.5861;
    }
}