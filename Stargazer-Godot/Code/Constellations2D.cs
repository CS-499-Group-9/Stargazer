using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stargazer
{

    /// <summary>
    /// The <see cref="Node2D"/> used to contain the constellation stars and lines in the viewport.
    /// Author: William Arnett
    /// Created: SPR 2025
    /// </summary>
    public partial class Constellations2D : Node2D
    {
        /// <summary>
        /// The scene used to instantiate the stars in the constellation
        /// </summary>
        [Export] public PackedScene Star2DScene { get; set; }
        /// <summary>
        /// The scene used to instantiate the labels in the constellation
        /// </summary>
        [Export] public PackedScene Label2DScene { get; set; }

        private Node2D StarContainer;
        private MeshInstance2D constMesh;
        private ImmediateMesh mesh;
        private Node2D ConstellationLabels;

        /// <summary>
        /// Gathers references to child nodes and initializes properties of the view.
        /// </summary>
        public override void _Ready()
        {
            // Get references to child objects
            StarContainer = GetNode<Node2D>("Star2DContainer");
            constMesh = GetNode<MeshInstance2D>("LineMesh");
            mesh = (ImmediateMesh)constMesh.Mesh;

            StandardMaterial3D whiteMaterial = new StandardMaterial3D();
            // Create a white material
            whiteMaterial.AlbedoColor = new Color(0.8f, 0.8f, 0.8f, 0.8f); // White color
            whiteMaterial.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
            // Assign the material to the mesh
            constMesh.Material = whiteMaterial;
            ConstellationLabels = GetNode<Node2D>("LineMesh/Constellation2DLabels");

        }

        /// <summary>
        /// Initially draws the constellations
        /// Refactored by Josh Johner (SPR 2025) to use the IDictionary.
        /// </summary>
        /// <param name="constellations">Contains the constellation graphs to draw.</param>
        /// <param name="DrawnStars">The dictionary of drawn 2D stars.</param>
        /// <returns></returns>
        public void DrawConstellations(IEnumerable<Constellation> constellations, IDictionary<int, Star2D> DrawnStars)
        {
            //var constellations = dataPackage.Constellations;
            mesh.ClearSurfaces();
            mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, constMesh.Material);
            Vector2 labelPos = new Vector2();

            foreach (var constellation in constellations)
            {
                Vector2 totalPos = new Vector2(0, 0);
                int c = 0;

                foreach (var lines in constellation.ConstellationLines)
                {
                    DrawnStars.TryGetValue(lines.Item1, out Star2D s1);
                    DrawnStars.TryGetValue(lines.Item2, out Star2D s2);
                    if (s1.Altitude > -45.0 || s2.Altitude > -45.0)
                    {
                        s1.Scale = new Vector2(0.6f, 0.6f); 
                        s2.Scale = new Vector2(0.6f, 0.6f);

                        mesh.SurfaceAddVertex(new Vector3(s1.Position[0], s1.Position[1], 0.0F));
                        mesh.SurfaceAddVertex(new Vector3(s2.Position[0], s2.Position[1], 0.0F));


                    }
                }
            }
            mesh.SurfaceEnd();
        }

    }
}