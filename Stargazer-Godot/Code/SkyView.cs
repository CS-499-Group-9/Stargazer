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
    public partial class SkyView : Node3D
    {
        [Export] private PackedScene MoonScene;
        [Export] private PackedScene SunScene;
        [Export] private Spawner StarSpawner;
        [Export] private MessierSpawner MessierSpawner;
        [Export] private Planets PlanetSpawner;
        [Export] private Constellations constellationNode;
        [Export] private HoverLabel HoverLabel;
        [Export] private AzimuthGridlines AzimuthGridlines;
        [Export] private GridLabel GridLabel;
        [Export] private CompassNeedle CompassNeedle;
        [Export] public MainCamera CameraStateNotifier;
        [Export] private ASTRALPLANEAlbedo AstralPlane;
        [Export] private Label dateLabel;
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
        private double averageFrameTime;

        private Planets planetNode;
        private Moon moon;
        private Sun sun;
        private IEquatorialCalculator calculator;
        
        private PlaySpeed playSpeed;
        private ulong previousTicks;

        /// <summary>
        /// Wires up communication between the different objects.
        /// </summary>
        public override void _Ready()
        {
            instantaneousFrameLabel = GetNode<Label>("InstantaneousFrameLabel");
            averageFrameLabel = GetNode<Label>("AverageFrameLabel");
            localSiderealLabel = GetNode<Label>("LocalSiderealLabel");

            // Subscribe all camera notifications
            CameraStateNotifier.OnRotation += CompassNeedle.RotationHandler;
            CameraStateNotifier.OnRotation += GridLabel.HandleCameraRotationChanged;
            CameraStateNotifier.OnRotation += AstralPlane.HandleCameraRotationChanged;
            CameraStateNotifier.OnZoomStateChanged += AzimuthGridlines.HandleZoomStateChanged;
            CameraStateNotifier.OnZoomStateChanged += GridLabel.HandleZoomStateChanged;
            CameraStateNotifier.OnHoverableChange += HoverLabel.HoverableChangeHandler;

            // Notify objects of the cameras initial position.
            CameraStateNotifier.OnRotation(CameraStateNotifier);
            
            // Subscribe all toggle notifications
            ToggleConstellationLines = constellationNode.ToggleConstellationLines;
            ToggleConstellationLabels = constellationNode.ToggleConstellationLabels;
            ToggleGridlines += AzimuthGridlines.ToggleGridlines;
            ToggleEquatorialGridlines += AzimuthGridlines.ToggleEquatorialGridlines;
            ToggleMessierObjects += MessierSpawner.ToggleMessierObjects;
            ToggleGridlines += GridLabel.ToggleGridlines;            
        }

        /// <summary>
        /// Controls the <see cref="IEquatorialCalculator"/> used by all objects.
        /// </summary>
        /// <param name="delta">Not used (the value does not account for time taken for calculations during the frame).</param>
        public override void _Process(double delta)
        {
            instantaneousFrameLabel.Text = $"Instantaneous: {delta.ToString()}";
            averageFrameTime += delta;
            averageFrameTime /= 2;
            averageFrameLabel.Text = $"Average: {averageFrameTime.ToString()}";
            localSiderealLabel.Text = calculator?.LST.ToString() ?? "Local Sidereal Time";

            int timeMultiplier = playSpeed?.TotalSeconds ?? 1;
            // Get the total number of seconds since the simulation has started.
            var totalTicks = Time.GetTicksMsec();
            // Get the total number of seconds that have elapsed since the last iteration
            double secSinceLast = (totalTicks - previousTicks)/1000.0;

            if (playSpeed?.IsSyncronized ?? false) 
            {
                // Update the calcuator using current time
                var timeNow = DateTime.UtcNow;
                calculator?.SetTime(timeNow);
            }
            else 
            {
                // Update the calculator using the seconds elapsed since the last iteration
                calculator?.IncrementTimeBy(secSinceLast * timeMultiplier);
            }
            
            previousTicks = totalTicks;
            dateLabel.Text = $"{calculator?.CurrentTime.ToLocalTime().ToString() ?? ""} Local";
        }

        /// <summary>
        /// Stores the <see cref="PlaySpeed"/> state object
        /// </summary>
        /// <param name="playSpeed">The object</param>
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
            dateLabel.Text = calculator.CurrentTime.ToString();
         
            // Draw all the objects
            await StarSpawner.DrawStars(dataPackage.HorizontalStars, dataPackage.GetStar,calculator);
            await constellationNode.DrawConstellations(dataPackage.Constellations, dataPackage.GetStar, StarSpawner.SpawnStar);
            await MessierSpawner.DrawMessierObjects(dataPackage.MessierObjects, calculator);
            
            PlanetSpawner.DrawPlanets(dataPackage.Planets, calculator);
            
            sun = SunScene.Instantiate<Sun>();
            sun.FromHorizontal(dataPackage.Sun, calculator);
            AddChild(sun);

            moon = MoonScene.Instantiate<Moon>();
            moon.FromHorizontal(dataPackage.Moon, calculator);
            moon.SetSun(sun);
            AddChild(moon);

            previousTicks = Time.GetTicksMsec();
        }


    }
}