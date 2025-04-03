using System;
using Godot;
namespace Stargazer
{
    public partial interface ICameraStateNotifier
    {

        public Action<ZoomState> OnZoomStateChanged { get; set; }
        public Action<float> OnZoom { get; set; }

        public ITrackable TrackedBody { get; }

        public IHoverable HoverBody { get; }

        public Action<Camera3D> OnRotation { get; set; }
    }
}
