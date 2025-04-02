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
        public Action<bool> ToggleEquatorialGridlines;
        /// <summary>
        /// Relays the user request to toggle the visibility of the Messier Objects to the node that makes the change
        /// </summary>
        public Action<bool> ToggleMessierObjects;
        public ICameraStateNotifier cameraStateNotifier {  get; set; }
        

        [Export] private Spawner spawner;
        [Export] private Spawner2D spawner2d;
        [Export] private Constellations constellationNode;
        [Export] private Constellations2D constellation2dNode;

       

        private Label averageFrameLabel;
        private Label instantaneousFrameLabel;
        private Label dateLable;
        private double averageFrameTime;

        private Planets planetNode;
        private Moon moon;
        private IEquatorialCalculator starConverter;
        
        private PlaySpeed playSpeed;
        private ulong previousTicks;

        /// <summary>
        /// Gathers references to child nodes and connects <see cref="Delegate"/>s to facilitate communication.
        /// </summary>
        public override void _Ready()
        {
            base._Ready();
            //spawner = GetNode<Spawner>("Stars");
            //constellationNode = GetNode<Constellations>("Constellations");
            cameraStateNotifier = GetNode<MainCamera>("Camera3D");
            var azimuthGridlines = GetNode<AzimuthGridlines>("Dome/Azimuth Gridlines");
            var gridLabel = GetNode<GridLabel>(nameof(GridLabel));
            var needle = GetNode<CompassNeedle>("Compass/Needle");


            cameraStateNotifier.OnRotation += needle.RotationHandler;

            dateLable = GetNode<Label>("TimeLabel");
            
            instantaneousFrameLabel = GetNode<Label>("InstantaneousFrameLabel");
            averageFrameLabel = GetNode<Label>("AverageFrameLabel");
            
            ToggleConstellationLines = constellationNode.ToggleConstellationLines;
            ToggleConstellationLabels = constellationNode.ToggleConstellationLabels;
            ToggleGridlines += azimuthGridlines.ToggleGridlines;
            ToggleEquatorialGridlines += azimuthGridlines.ToggleEquatorialGridlines;
            
            cameraStateNotifier.OnZoomStateChanged += azimuthGridlines.HandleZoomStateChanged;
            cameraStateNotifier.OnZoomStateChanged += gridLabel.HandleZoomStateChanged;
            cameraStateNotifier.OnRotation += gridLabel.HandleCameraRotationChanged;
            
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
           
            int timeMultiplier = playSpeed?.TotalSeconds ?? 1;
            var totalTicks = Time.GetTicksMsec();
            double secSinceLast = (totalTicks - previousTicks)/1000.0;
            if (playSpeed?.IsSyncronized ?? false) 
            {
                var timeNow = DateTime.UtcNow;
                starConverter?.SetTime(timeNow);
            }
            else 
            {
                starConverter?.IncrementTimeBy(secSinceLast * timeMultiplier);
            }
            
            previousTicks = totalTicks;
            dateLable.Text = $"{starConverter?.CurrentTime.ToLocalTime().ToString() ?? ""} Local";
            base._Process(delta);
        }

        public void SetTimeMultiplier(PlaySpeed playSpeed)
        {
            this.playSpeed = playSpeed;
        }


        /// <summary>
        /// Notifies child notes of a new <see cref="CelestialDataPackage{Star}"/> that is ready to be drawn.
        /// </summary>
        /// <param name="dataPackage">The <see cref="CelestialDataPackage{Star}"/></param>
        /// <returns><see cref="Task"/> that can be awaited.</returns>
        public async Task UpdateUserPosition(CelestialDataPackage<Star> dataPackage)
        {
            starConverter = dataPackage.Calculator;
            dateLable.Text = starConverter.CurrentTime.ToString();
            await spawner.DrawStars(dataPackage.HorizontalStars, dataPackage.GetStar,starConverter);
            await constellationNode.DrawConstellations(dataPackage.Constellations, dataPackage.GetStar, spawner.SpawnStar);
            planetNode.DrawPlanets(dataPackage.Planets, starConverter);
            moon?.Free();
            moon = MoonScene.Instantiate<Moon>();
            moon.FromHorizontal(dataPackage.Moon, starConverter);
            previousTicks = Time.GetTicksMsec();
            AddChild(moon);
        }
    }
}