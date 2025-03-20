using DataLayer;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using Godot;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stargazer
{

    /// <summary>
    /// The parent object for all items in the viewport.
    /// Responsible for relaying notifications from senders throughout the program into the view port.
    /// </summary>
    public partial class SkyView : Node3D
    {
        /// <summary>
        /// Relays the user request to toggle the constellations lines down to the child node that makes the change
        /// </summary>
        public Action<bool> ToggleConstellationLines;
        /// <summary>
        /// Relays the user request to toggle the constellation labels to the child node that makes the change
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

        private Spawner spawner;
        private Spawner2D spawner2d;
        private Constellations constellationNode;
        private Constellations2D constellation2dNode;

        /// <summary>
        /// Gathers references to child nodes and connects <see cref="Delegate"/>s to facilitate communication.
        /// </summary>
        public override void _Ready()
        {
            base._Ready();
            spawner = GetNode<Spawner>("Stars");
            spawner2d = GetNode<Spawner2D>("/root/Control/SubViewport2/View2d/Stars2D");
            constellationNode = GetNode<Constellations>("Constellations");
            constellation2dNode = GetNode<Constellations2D>("/root/Control/SubViewport2/View2d/Constellations2D");
            var azimuthGridlines = GetNode<AzimuthGridlines>("Dome/Azimuth Gridlines");
            ToggleConstellationLines = constellationNode.ToggleConstellationLines;
            ToggleConstellationLabels = constellationNode.ToggleConstellationLabels;
            ToggleGridlines = azimuthGridlines.ToggleGridlines;
            // TODO: Get a reference to the Messier Objects parent node (should be a child of this node)
            // TODO: Get a reference to the Moon object parent node (should be a child of this node) 
            // TODO: Get a reference to the Planets object parent node (should be a child of this node)
        }

        /// <summary>
        /// Notifies child notes of a new <see cref="CelestialDataPackage{Star}"/> that is ready to be drawn.
        /// </summary>
        /// <param name="dataPackage">The <see cref="CelestialDataPackage{Star}"/></param>
        /// <returns><see cref="Task"/> that can be awaited.</returns>
        public async Task UpdateUserPosition(CelestialDataPackage<Star> dataPackage)
        {
            var count = 0;
            var nonnullcount = 0;
            //GD.Print($"nullcount {count}\nnonnullcount {nonnullcount}");
            await spawner.DrawStars(dataPackage.Stars);
            await spawner2d.DrawStars(dataPackage.Stars);
            await constellationNode.DrawConstellations(dataPackage.Constellations, dataPackage.GetConstellationStar);
            await constellation2dNode.DrawConstellations(dataPackage.Constellations, dataPackage.GetConstellationStar);
            // TODO: Notify the Messier Objects node to draw the Messier Objects
            // TODO: Notify the Moon node to draw the moon
            // TODO: Notify the Planets node to draw the planets
        }

    }
}