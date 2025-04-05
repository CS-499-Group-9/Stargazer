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
        /// </summary>
        /// <param name="constellations">Contains the constellation graphs to draw.</param>
        /// <param name="GetConstellationStar">A method to retrieve a star by Hipparcos Id from the dictionary of drawn stars.</param>
        /// <returns></returns>
        public async Task DrawConstellations(IEnumerable<Constellation> constellations, Func<int, Func<HorizontalStar, Star>, Star> GetConstellationStar)
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
                    Star s1 = GetConstellationStar(lines.Item1, (S) => { return null; });
                    Star s2 = GetConstellationStar(lines.Item2, (S) => { return null; });
                    if (s1.Altitude > -45.0 || s2.Altitude > -45.0)
                    {
                        Star2D s12d = Spawn2DStar(s1);
                        Star2D s22d = Spawn2DStar(s2);

                        mesh.SurfaceAddVertex(new Vector3(s12d.Position[0], s12d.Position[1], 0.0F));
                        mesh.SurfaceAddVertex(new Vector3(s22d.Position[0], s22d.Position[1], 0.0F));

                        if (totalPos == Vector2.Zero) // solely checked for the first star
                        {
                            totalPos += s1.Position2D;
                            c++;
                        }
                        totalPos += s2.Position2D;
                        c++;
                    }
                }
            }
            mesh.SurfaceEnd();
            foreach (Star2D star in StarContainer.GetChildren())
            {
                star.Scale = new Vector2(0.6f, 0.6f);
            }
        }

        /// <summary>
        /// Receives the notification to toggle the visibility of the constellation lines.
        /// Hiding the lines will also hide the labels.
        /// </summary>
        /// <param name="showlines">True if the user has requested to show the lines.</param>
        public void ToggleConstellationLines(bool showlines)
        {
            constMesh.Visible = showlines;
        }

        /// <summary>
        /// Receives the notification to toggle the visibility of the constellation labels.
        /// </summary>
        /// <param name="showlabels">True if the user has requested to show the labels.</param>
        public void ToggleConstellationLabels(bool showlabels) { ConstellationLabels.Visible = showlabels; }

        private Star2D Spawn2DStar(Star star3d)
        {
            Star2D outstar = Star2DScene.Instantiate<Star2D>();
            outstar.From3dStar(star3d);
            outstar.scalestar(new Vector2(0.7f, 0.7f));
            //outstar.Scale  = new Vector2(1F,1F);
            StarContainer.AddChild(outstar);
            return outstar;
        }
    }
}