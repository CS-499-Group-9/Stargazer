using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;

namespace Stargazer
{
    /// <summary>
    /// Used to draw a planet in the <see cref="SkyView"/>
    /// </summary>
    public partial class Planet : CelestialBody
    {

        private HorizontalPlanet horizontalPlanet;
        private Texture2D planetTexture;
        private ShaderMaterial planetMaterial;
        private Globals globalVars;
        // Called when the node enters the scene tree for the first time.

        /// <summary>
        /// Used to initialize planetary data.
        /// </summary>
        public override void _Ready()
        {
            globalVars = GetNode<Globals>("/root/Globals"); // Import globals

        }




        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            calculator?.UpdatePositionOf(horizontalPlanet);
            globalVars.LocalSiderealTime = (double)(calculator?.LST);
            Position = GetLocation();
            Rotate(Vector3.Up,Mathf.Pi);
            RotationDegrees = new Vector3(0,0,-90+34.7304f);
        }

        public void setTexture(Resource planettexture)
        {
            var planetMesh = GetNode<MeshInstance3D>("PlanetBody/PlanetMesh");
            planetMaterial = (ShaderMaterial)planetMesh.GetSurfaceOverrideMaterial(0).Duplicate();
            planetMesh.SetSurfaceOverrideMaterial(0,planetMaterial);
            planetMaterial.SetShaderParameter("albedo_texture",planettexture);
        }

        /// <summary>
        /// Used to receive the data and methods to perform calculations
        /// </summary>
        /// <param name="horizonalPlanet">Contains the data to perform the calculations</param>
        /// <param name="calculator">The calculator used.</param>
        public void FromHorizontal(HorizontalPlanet horizonalPlanet, IEquatorialCalculator calculator)
        {
            base.FromHorizontal(horizonalPlanet, calculator);  
            horizontalPlanet = horizonalPlanet;
            this.calculator = calculator;
        }

        /// <inheritdoc/>
        public override string GetHoverText()
        {
            String planetName;
            if (horizontalPlanet.Name.Equals("Sun"))
            {
                planetName = "The Sun";
            }
            else if (String.IsNullOrWhiteSpace(horizontalPlanet.Name))
            {
                planetName = "Unnamed Planet";
            }
            else
            {
                planetName = horizontalPlanet.Name;
            }
            return $"{planetName}\n" +
            $"Altitude: {horizontalPlanet.Altitude}\n" +
            $"Azimuth: {horizontalPlanet.Azimuth}\n" +
            $"Distance: {horizontalPlanet.Distance}";
        }
        
        
        private Texture2D LoadTexture(string path)
        {
            Image image = new Image();
            Error err = image.Load(path);

            if (err != Error.Ok)
            {
                GD.PrintErr($"Failed to load image: {path}");
                return null;
            }

            ImageTexture texture = ImageTexture.CreateFromImage(image);
            return texture;
        }

    }

}