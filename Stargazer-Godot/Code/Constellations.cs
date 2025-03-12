using DataLayer;
using DataLayer.HorizontalObjects;
using Godot;
using System;

namespace Stargazer
{

    /// <summary>
    /// The <see cref="Node3D"/> used to contain the constellation stars and lines in the viewport.
    /// </summary>
    public partial class Constellations : Node3D
    {
        /// <summary>
        /// The scene used to instantiate the stars in the constellation
        /// </summary>
        [Export] public PackedScene StarScene { get; set; }
        /// <summary>
        /// The scene used to instantiate the labels in the constellation
        /// </summary>
        [Export] public PackedScene LabelScene { get; set; }

        private Node3D StarContainer;
        private MeshInstance3D constMesh;
        private ImmediateMesh mesh;
        private Node3D ConstellationLabels;

        /// <summary>
        /// Gathers references to child nodes and initializes properties of the view.
        /// </summary>
        public override void _Ready()
        {
            // Get references to child objects
            StarContainer = GetNode<Node3D>("StarContainer");
            constMesh = GetNode<MeshInstance3D>("LineMesh");
            mesh = (ImmediateMesh)constMesh.Mesh;

            StandardMaterial3D whiteMaterial = new StandardMaterial3D();
            // Create a white material
            whiteMaterial.AlbedoColor = new Color(0.8f, 0.8f, 0.8f, 0.8f); // White color
            whiteMaterial.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
            // Assign the material to the mesh
            constMesh.MaterialOverride = whiteMaterial;
            ConstellationLabels = GetNode<Node3D>("LineMesh/ConstellationLabels");

            base._Ready();

        }

        /// <summary>
        /// Passed as a <see cref="Delegate"/> to <see cref="Startup.UserPositionUpdated"/> to be notified when a new star scene is ready to be drawn.
        /// This should be done using the += operator to be notified in addition to other components in the viewport.
        /// </summary>
        /// <param name="dataPackage"></param>
        public void DrawConstellations(CelestialDataPackage<Star> dataPackage)
        {
            var constellations = dataPackage.Constellations;
            mesh.ClearSurfaces();
            foreach (var star in StarContainer.GetChildren()) { star.Free(); }
            foreach (var line in ConstellationLabels.GetChildren()) { line.Free(); }
            mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, constMesh.MaterialOverride);
            Vector3 labelPos = new Vector3();

            foreach (var constellation in constellations)
            {
                Vector3 totalPos = new Vector3(0, 0, 0);
                int c = 0;
                GD.Print($"Drawing constellation {constellation.ConstellationName}");

                foreach (var lines in constellation.ConstellationLines)
                {

                    Star s1 = dataPackage.GetConstellationStar(lines.Item1, SpawnStar);
                    Star s2 = dataPackage.GetConstellationStar(lines.Item2, SpawnStar);

                    mesh.SurfaceAddVertex(s1.Position);
                    mesh.SurfaceAddVertex(s2.Position);
                    if (totalPos == Vector3.Zero) // solely checked for the first star
                    {
                        totalPos += s1.Position;
                        c++;
                    }
                    totalPos += s2.Position;
                    c++;
                }

                // Creating labels
                labelPos = totalPos / c;

                LabelNode labelNode = LabelScene.Instantiate<LabelNode>();
                labelNode.LabelText = constellation.ConstellationName;
                labelNode.Position = labelPos;
                labelNode.Visible = true;
                ConstellationLabels.AddChild(labelNode);

            }
            mesh.SurfaceEnd();
            var count = constMesh.Mesh.GetSurfaceCount();
            GD.Print(count);
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

        private Star SpawnStar(HorizontalStar horizontalStar)
        {
            Star star = StarScene.Instantiate<Star>();
            star.azimuth = (float)horizontalStar.Azimuth;
            star.altitude = (float)horizontalStar.Altitude;
            star.mag = (float)horizontalStar.Magnitude;
            star.starName = horizontalStar.StarName;

            StarContainer.AddChild(star);
            return star;
        }
    }
}