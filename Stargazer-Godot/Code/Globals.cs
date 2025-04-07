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
        public DateTime requestTime { get; set; }

        public double LocalSiderealTime{get; set;}

        public double LatitudePass { get; set; }
        public double LongitudePass { get; set; }
        public DateTime TimePass { get; set; }


        public override void _Ready()
        {
            Instance = this;
            isAzimuth = false;
            isConstellation = true;
            isLabel = true;
            isHover = false;
            hoverLabel = "";
        }

        public void UpdatePosition(double latitude, double longitude, DateTime time)
        {
            LatitudePass = latitude;
            LongitudePass= longitude;
            TimePass = time;
        }
    }
}