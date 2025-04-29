using Godot;

namespace Stargazer
{
    /// <summary>
    /// Provides the ability of an object to be tracked by the camera
    /// Author: Josh Johenr
    /// Created: SPR 2025
    /// </summary>
    public interface ITrackable
    {
        /// <summary>
        /// Exposes the objects <see cref="Node3D.GlobalTransform"/> for tracking.
        /// </summary>
        public Transform3D GlobalTransform { get; }
    }
}