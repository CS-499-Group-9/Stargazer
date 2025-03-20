using DataLayer;
using DataLayer.HorizontalObjects;
using Godot;
using System;

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
            mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, constMesh.Material);
            Vector2 labelPos = new Vector2();

            foreach (var constellation in constellations)
            {
                Vector2 totalPos = new Vector2(0, 0);
                int c = 0;
                //GD.Print($"Drawing constellation {constellation.ConstellationName}");

                foreach (var lines in constellation.ConstellationLines)
                {
                    //GD.Print("gonna grab a constellation");
                    Star s1 = dataPackage.GetConstellationStar(lines.Item1, (S)=>{return null;});
                    Star s2 = dataPackage.GetConstellationStar(lines.Item2, (S)=>{return null;});
                    if (s1.altitude > -45.0 || s2.altitude > -45.0){
                        Star2D s12d = Spawn2DStar(s1);
                        Star2D s22d = Spawn2DStar(s2);

                        mesh.SurfaceAddVertex(new Vector3(s12d.Position[0],s12d.Position[1],0.0F));
                        mesh.SurfaceAddVertex(new Vector3(s22d.Position[0],s22d.Position[1],0.0F));

                        if (totalPos == Vector2.Zero) // solely checked for the first star
                        {
                            totalPos += s1.Pos2D;
                            c++;
                        }
                        totalPos += s2.Pos2D;
                        c++;
                    }

                }

                // Creating labels
                // labelPos = totalPos / c;

                // LabelNode labelNode = LabelScene.Instantiate<LabelNode>();
                // labelNode.LabelText = constellation.ConstellationName;
                // labelNode.Position = new Vector3(labelPos[0],labelPos[1],0.0F);
                // labelNode.Visible = true;
                // ConstellationLabels.AddChild(labelNode);

            }
            mesh.SurfaceEnd();
            var count = constMesh.Mesh.GetSurfaceCount();
            foreach(Star2D star in StarContainer.GetChildren()){
                star.Scale = new Vector2(0.6f,0.6f);
            }
            //GD.Print(count);
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
            
            outstar.azimuth = star3d.azimuth;
            outstar.altitude = star3d.altitude;
            outstar.mag = star3d.mag;
            outstar.scalestar(new Vector2(1.0f,1.0f));
            //outstar.Scale  = new Vector2(1F,1F);
            outstar.starName = star3d.starName;
            StarContainer.AddChild(outstar);
            return outstar;
        }
    }
}