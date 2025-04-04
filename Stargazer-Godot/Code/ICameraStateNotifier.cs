using System;
using Godot;
namespace Stargazer
{
    /// <summary>
    /// Notifies subscribers of events related to the <see cref="Camera3D"/>
    /// </summary>
    public partial interface ICameraStateNotifier
    {
        /// <summary>
        /// Notifies subscribers when the <see cref="ZoomState"/> has changed
        /// </summary>
        public Action<ZoomState> OnZoomStateChanged { get; set; }
        /// <summary>
        /// Notifies subscribers when the <see cref="Camera3D.Fov"/> has changed.
        /// </summary>
        public Action<float> OnZoom { get; set; }

        /// <summary>
        /// Notifies subscribers when the camera has rotated.
        /// </summary>
        public Action<Camera3D> OnRotation { get; set; }
        /// <summary>
        /// Notifies subscribers when the mouse has entered or exited a hovered body.
        /// </summary>
        public Action<IHoverable> OnHoverableChange { get; set; }
    }
}
