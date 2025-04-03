using Godot;

namespace Stargazer
{
    public interface ITrackable
    {
        float Azimuth { get; }
        float Altitude { get; }

        public Transform3D GlobalTransform { get; }
    }
}