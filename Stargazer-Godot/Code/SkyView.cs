using DataLayer;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using Godot;
using System;
using System.Collections.Concurrent;

namespace Stargazer
{

    /// <summary>
    /// The parent object for all items in the viewport.
    /// Responsible for relaying notifications from senders throughout the program into the view port.
    /// </summary>
    public partial class SkyView : Node3D
    {
        /// <summary>
        /// Relays the updated user request from <see cref="Startup.UserPositionUpdated"/> to children which will handle drawing the objects.
        /// </summary>
        public Action<CelestialDataPackage<Star>> UpdateUserPosition;
        /// <summary>
        /// Relays the user request to toggle the constellations lines down to the child node that makes the change
        /// </summary>
        public Action<bool> ToggleConstellationLines;
        /// <summary>
        /// Relays the user request to toggle the constellation lables to the child node that makes the change
        /// </summary>
        public Action<bool> ToggleConstellationLabels;
        /// <summary>
        /// Relays the user request to toggle the gridlines to the child node that makes the change.
        /// </summary>
        public Action<bool> ToggleGridlines;
        /// <summary>
        /// Relays the user request to toggle the visibility of the Messier Objects to the node that makes the change
        /// </summary>
        public Action<bool> ToggleMessierObjects;

        /// <summary>
        /// Gathers references to child nodes and connects <see cref="Delegate"/>s to facilitate communication.
        /// </summary>
        public override void _Ready()
        {
            base._Ready();
            var spawner = GetNode<Spawner>("Stars");
            var spawner2d = GetNode<Spawner2D>("/root/Control/SubViewport2/View2d/Stars2D");
            UpdateUserPosition += spawner.DrawStars;
            UpdateUserPosition += spawner2d.DrawStars;
            var constellationNode = GetNode<Constellations>("Constellations");
            var constellation2dNode = GetNode<Constellations2D>("/root/Control/SubViewport2/View2d/Constellations2D");
            UpdateUserPosition += constellationNode.DrawConstellations;
            UpdateUserPosition += constellation2dNode.DrawConstellations;
            ToggleConstellationLines = constellationNode.ToggleConstellationLines;
            ToggleConstellationLabels = constellationNode.ToggleConstellationLabels;
            var azimuthGridlines = GetNode<AzimuthGridlines>("Dome/Azimuth Gridlines");
            ToggleGridlines = azimuthGridlines.ToggleGridlines;
            // TODO: Get a reference to the Messier Objects parent node (should be a child of this node) and wire up the delegate.

        }
    }
}