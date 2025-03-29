using DataLayer;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Stargazer
{

    /// <summary>
    /// The parent object for all items in the viewport.
    /// Responsible for relaying notifications from senders throughout the program into the view port.
    /// </summary>
    public partial class SkyView : Node3D, IUserUpdateReceiver
    {
        [Export] PackedScene MoonScene { get; set; }
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
        public Camera3D Camera { get; set; }


        private Spawner spawner;
        private Spawner2D spawner2d;
        private Constellations constellationNode;
        private Constellations2D constellation2dNode;
        private Planets planetNode;

        private Label averageFrameLabel;
        private Label instantaneousFrameLabel;
        private Label dateLable;

        private Moon moon;
        private IEquatorialCalculator<HorizontalStar> starConverter;
        private IPlanetaryCalculator<HorizontalPlanet> planetaryCalculator;
        private IMoonCalculator moonCalculator;
        private int timeMultiplier = 1;
        private double averageFrameTime;

        /// <summary>
        /// Gathers references to child nodes and connects <see cref="Delegate"/>s to facilitate communication.
        /// </summary>
        public override void _Ready()
        {
            base._Ready();
            spawner = GetNode<Spawner>("Stars");
            //spawner2d = GetNode<Spawner2D>("/root/Control/SubViewport2/View2d/Stars2D");
            constellationNode = GetNode<Constellations>("Constellations");
            //constellation2dNode = GetNode<Constellations2D>("/root/Control/SubViewport2/View2d/Constellations2D");
            var azimuthGridlines = GetNode<AzimuthGridlines>("Dome/Azimuth Gridlines");
            ToggleConstellationLines = constellationNode.ToggleConstellationLines;
            ToggleConstellationLabels = constellationNode.ToggleConstellationLabels;
            ToggleGridlines += azimuthGridlines.ToggleGridlines;
            dateLable = GetNode<Label>("TimeLabel");
            averageFrameLabel = GetNode<Label>("AverageFrameLabel");
            instantaneousFrameLabel = GetNode<Label>("InstantaneousFrameLabel");
            Camera = GetNode<Camera3D>("Camera3D");
            var needle = GetNode<CompassNeedle>("Compass/Needle");
            var gridLabel = GetNode<GridLabel>(nameof(GridLabel));
            azimuthGridlines.SetCamera(Camera);
            needle.SetCamera(Camera);
            gridLabel.SetCamera(Camera);
            ToggleGridlines += gridLabel.ToggleGridlines;

            planetNode = GetNode<Planets>("Planets");



            // TODO: Get a reference to the Messier Objects parent node (should be a child of this node)
            // TODO: Get a reference to the Moon object parent node (should be a child of this node) 
        }

        public override void _Process(double delta)
        {
            instantaneousFrameLabel.Text = $"Instantaneous: {delta.ToString()}";
            averageFrameTime += delta;
            averageFrameTime /= 2;
            averageFrameLabel.Text = $"Average: {averageFrameTime.ToString()}";

            starConverter?.IncrementTimeBy(delta * timeMultiplier);
            planetaryCalculator?.IncrementTimeBy(delta * timeMultiplier);
            moonCalculator?.IncrementTimeBy(delta * timeMultiplier);
            dateLable.Text = $"{starConverter?.CurrentTime.ToLocalTime().ToString() ?? ""} Local";
            base._Process(delta);
        }

        public int UpdatePlaySpeed(int multiplier)
        {
            if (multiplier == 0) timeMultiplier = 1;
            else timeMultiplier += multiplier;
            return timeMultiplier;
        }

        public void SyncronizeTime()
        {
            var currentTime = DateTime.UtcNow;
            starConverter.SetTime(currentTime);
            planetaryCalculator?.SetTime(currentTime);
            moonCalculator?.SetTime(currentTime);
            dateLable.Text = $"{currentTime.ToLocalTime().ToString() ?? ""} Local";
            timeMultiplier = 1;
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
            starConverter = dataPackage.StarCalculator;
            planetaryCalculator = dataPackage.PlanetaryCalculator;
            dateLable.Text = starConverter.CurrentTime.ToString();
            //GD.Print($"nullcount {count}\nnonnullcount {nonnullcount}");
            await spawner.DrawStars(dataPackage.HorizontalStars, dataPackage.GetStar, dataPackage.StarCalculator);
            //await spawner2d.DrawStars(dataPackage.Stars);
            await constellationNode.DrawConstellations(dataPackage.Constellations, dataPackage.GetStar, spawner.SpawnStar);
            planetNode.DrawPlanets(dataPackage.Planets, dataPackage.PlanetaryCalculator);
            moon?.Free();
            moonCalculator = dataPackage.MoonCalculator;
            moon = MoonScene.Instantiate<Moon>();
            moon.FromHorizontal(dataPackage.Moon, moonCalculator);
            AddChild(moon);
            //await constellation2dNode.DrawConstellations(dataPackage.Constellations, dataPackage.GetConstellationStar);
            // TODO: Notify the Messier Objects node to draw the Messier Objects
        }
    }
}