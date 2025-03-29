using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;

namespace Stargazer
{
    /// <summary>
    /// Used to draw a planet in the <see cref="SkyView"/>
    /// </summary>
    public partial class Planet : Node3D, IHoverable
    {

        private IPlanetaryCalculator<HorizontalPlanet> calculator;
        private HorizontalPlanet horizonalPlanet;
        private float Distance = 74f;
        private const float radians = (float)Math.PI / 180f;
        private Texture2D[] planetTextureArray;
        // Called when the node enters the scene tree for the first time.

        /// <summary>
        /// Used to initialize planetary data.
        /// </summary>
        public override void _Ready()
        {
            planetTextureArray = new Texture2D[8];
            planetTextureArray[0] = LoadTexture("res://Textures/mercurymap.jpg");
            planetTextureArray[1] = LoadTexture("res://Textures/venusmap.jpg");
            planetTextureArray[2] = LoadTexture("res://Textures/marsmap.jpg");
            planetTextureArray[3] = LoadTexture("res://Textures/jupitermap.jpg");
            planetTextureArray[4] = LoadTexture("res://Textures/saturnmap.jpg");
            planetTextureArray[5] = LoadTexture("res://Textures/uranusmap.jpg");
            planetTextureArray[6] = LoadTexture("res://Textures/neptunemap.jpg");
            planetTextureArray[7] = LoadTexture("res://Textures/sunmap.jpg");
            var planetMesh = GetNode<MeshInstance3D>("PlanetBody/PlanetMesh");
            ShaderMaterial planetMaterial = (ShaderMaterial)planetMesh.GetSurfaceOverrideMaterial(0).Duplicate();
            planetMesh.SetSurfaceOverrideMaterial(0, planetMaterial);
            if (horizonalPlanet.Name.Equals("Mercury"))
            {
                Scale = new Vector3(3, 3, 3);
                planetMaterial.SetShaderParameter("albedo_texture", planetTextureArray[0]);
            }
            else if (horizonalPlanet.Name.Equals("Venus"))
            {
                Scale = new Vector3(3, 3, 3);
                planetMaterial.SetShaderParameter("albedo_texture", planetTextureArray[1]);
            }
            else if (horizonalPlanet.Name.Equals("Mars"))
            {
                Scale = new Vector3(3, 3, 3);
                planetMaterial.SetShaderParameter("albedo_texture", planetTextureArray[2]);
            }
            else if (horizonalPlanet.Name.Equals("Jupiter"))
            {
                Scale = new Vector3(3, 3, 3);
                planetMaterial.SetShaderParameter("albedo_texture", planetTextureArray[3]);
            }
            else if (horizonalPlanet.Name.Equals("Saturn"))
            {
                Scale = new Vector3(3, 3, 3);
                planetMaterial.SetShaderParameter("albedo_texture", planetTextureArray[4]);
            }
            else if (horizonalPlanet.Name.Equals("Uranus"))
            {
                Scale = new Vector3(3, 3, 3);
                planetMaterial.SetShaderParameter("albedo_texture", planetTextureArray[5]);
            }
            else if (horizonalPlanet.Name.Equals("Neptune"))
            {
                Scale = new Vector3(3, 3, 3);
                planetMaterial.SetShaderParameter("albedo_texture", planetTextureArray[6]);
            }
            else if (horizonalPlanet.Name.Equals("Sun"))
            {
                Scale = new Vector3(5, 5, 5);
                planetMaterial.SetShaderParameter("albedo_texture", planetTextureArray[7]);
            }

        }

        /// <summary>
        /// Calculates the position and phase of the planet each frame.
        /// </summary>
        /// <param name="delta">Unused</param>
        public override void _Process(double delta)
        {
            calculator?.UpdatePositionOf(horizonalPlanet);
            Position = GetLocation();
        }

        private Vector3 GetLocation()
        {
            var altRad = (float)horizonalPlanet.Altitude * radians;
            var azRad = (float)horizonalPlanet.Azimuth * radians;
            Vector3 pos = new()
            {
                X = Distance * (Mathf.Cos(azRad) * Mathf.Cos(altRad)),
                Y = Distance * Mathf.Sin(altRad),
                Z = Distance * Mathf.Cos(altRad) * Mathf.Sin(azRad)
            };
            return pos;
        }

        /// <summary>
        /// Used to receive the data and methods to perform calculations
        /// </summary>
        /// <param name="horizonalPlanet">Contains the data to perform the calculations</param>
        /// <param name="calculator">The calculator used.</param>
        public void FromHorizontal(HorizontalPlanet horizonalPlanet, IPlanetaryCalculator<HorizontalPlanet> calculator)
        {
            this.horizonalPlanet = horizonalPlanet;
            this.calculator = calculator;
        }

        /// <inheritdoc/>
        public string GetHoverText()
        {
            String planetName;
            if (horizonalPlanet.Name.Equals("Sun"))
            {
                planetName = "The Sun";
            }
            else if (String.IsNullOrWhiteSpace(horizonalPlanet.Name))
            {
                planetName = "Unnamed Planet";
            }
            else
            {
                planetName = horizonalPlanet.Name;
            }
            return $"{planetName}\n" +
            $"Altitude {horizonalPlanet.Altitude}\n" +
            $"Azimuth {horizonalPlanet.Azimuth}";
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