using Godot;
using System;
namespace Stargazer
{
    /// <summary>
    /// The parent container of the <see cref="Stargazer.SkyView"/> object (used for anchoring).
    /// </summary>
    public partial class SkyViewContainer : SubViewportContainer
    {
        /// <summary>
        /// Retrieves a reference to the <see cref="Stargazer.SkyView"/>
        /// </summary>
        [Export] public SkyView SkyView { get; set; }
        // Called when the node enters the scene tree for the first time.

        public Action<IHoverable> OnHoverableChange { get; set; }

        /// <inheritdoc/>
        public override void _Ready()
        {
            base._Ready();
            OnHoverableChange = SkyView.CameraStateNotifier.OnHoverableChange;
        }

    }
}