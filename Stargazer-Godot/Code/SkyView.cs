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
        [Export] private PackedScene MoonScene;
        [Export] private PackedScene SunScene;
        [Export] private Spawner spawner;
        [Export] private MessierSpawner MessierSpawner;
        [Export] private Constellations constellationNode;
        [Export] private HoverLabel HoverLabel;
        [Export] private AzimuthGridlines AzimuthGridlines;
        [Export] private GridLabel GridLabel;
        [Export] private CompassNeedle CompassNeedle;
        [Export] public MainCamera CameraStateNotifier;
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
        
        

       

        private Label averageFrameLabel;
        private Label instantaneousFrameLabel;
        private Label localSiderealLabel;
        private Label dateLable;
        private double averageFrameTime;

        private Planets planetNode;
        private Moon moon;
        private Sun sun;
        private IEquatorialCalculator calculator;
        
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
            
                     

            CameraStateNotifier.OnRotation += CompassNeedle.RotationHandler;
            CameraStateNotifier.OnHoverableChange += HoverLabel.HoverableChangeHandler;
            dateLable = GetNode<Label>("TimeLabel");
            
            instantaneousFrameLabel = GetNode<Label>("InstantaneousFrameLabel");
            averageFrameLabel = GetNode<Label>("AverageFrameLabel");
            localSiderealLabel = GetNode<Label>("LocalSiderealLabel");

            ToggleConstellationLines = constellationNode.ToggleConstellationLines;
            ToggleConstellationLabels = constellationNode.ToggleConstellationLabels;
            ToggleGridlines += AzimuthGridlines.ToggleGridlines;
            ToggleEquatorialGridlines += AzimuthGridlines.ToggleEquatorialGridlines;
            
            CameraStateNotifier.OnZoomStateChanged += AzimuthGridlines.HandleZoomStateChanged;
            CameraStateNotifier.OnZoomStateChanged += GridLabel.HandleZoomStateChanged;
            CameraStateNotifier.OnRotation += GridLabel.HandleCameraRotationChanged;
            
            ToggleGridlines += GridLabel.ToggleGridlines;
            
            planetNode = GetNode<Planets>("Planets");
            CameraStateNotifier.OnRotation(CameraStateNotifier);


            // TODO: Get a reference to the Messier Objects parent node (should be a child of this node)
            // TODO: Get a reference to the Moon object parent node (should be a child of this node) 
        }

        public override void _Process(double delta)
        {
            instantaneousFrameLabel.Text = $"Instantaneous: {delta.ToString()}";
            averageFrameTime += delta;
            averageFrameTime /= 2;
            averageFrameLabel.Text = $"Average: {averageFrameTime.ToString()}";
            localSiderealLabel.Text = calculator?.LST.ToString() ?? "Local Sidereal Time";
            int timeMultiplier = playSpeed?.TotalSeconds ?? 1;
            var totalTicks = Time.GetTicksMsec();
            double secSinceLast = (totalTicks - previousTicks)/1000.0;
            if (playSpeed?.IsSyncronized ?? false) 
            {
                var timeNow = DateTime.UtcNow;
                calculator?.SetTime(timeNow);
            }
            else 
            {
                calculator?.IncrementTimeBy(secSinceLast * timeMultiplier);
            }
            
            previousTicks = totalTicks;
            dateLable.Text = $"{calculator?.CurrentTime.ToLocalTime().ToString() ?? ""} Local";
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
        public async Task InitializeCelestial(CelestialDataPackage<Star> dataPackage)
        {
            calculator = dataPackage.Calculator;
            AzimuthGridlines.SetCalculator(calculator);
            dateLable.Text = calculator.CurrentTime.ToString();
            await spawner.DrawStars(dataPackage.HorizontalStars, dataPackage.GetStar,calculator);
            await constellationNode.DrawConstellations(dataPackage.Constellations, dataPackage.GetStar, spawner.SpawnStar);
            await MessierSpawner.DrawMessierObjects(dataPackage.MessierObjects, calculator);
            planetNode.DrawPlanets(dataPackage.Planets, calculator);
            moon?.Free();
            moon = MoonScene.Instantiate<Moon>();
            moon.FromHorizontal(dataPackage.Moon, calculator);
            sun = SunScene.Instantiate<Sun>();
            sun.FromHorizontal(dataPackage.Sun, calculator);
            previousTicks = Time.GetTicksMsec();
            AddChild(moon);
            AddChild(sun);
        }
    }
}