using DataLayer;
using DataLayer.HorizontalObjects;
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

public partial class Constellations : Node3D
{
    [Export] public PackedScene StarScene { get; set; }
    [Export] public PackedScene LabelScene { get; set; }
    internal Startup Startup { get; set; }
    private MeshInstance3D constMesh;
    private ImmediateMesh mesh;
    private Node3D ConstellationLabels;

    public override void _Ready()
    {
       
        constMesh = GetNode<MeshInstance3D>("LineMesh");
        mesh = (ImmediateMesh)constMesh.Mesh;
        StandardMaterial3D whiteMaterial = new StandardMaterial3D();
        // Create a white material
        whiteMaterial.AlbedoColor = new Color(0.8f, 0.8f, 0.8f, 0.8f); // White color
        whiteMaterial.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        // Assign the material to the mesh
        constMesh.MaterialOverride = whiteMaterial;
        ConstellationLabels = GetNode<Node3D>("ConstellationLabels");

        base._Ready();

    }

    public void DrawConstellations(object source, CelestialDataPackage<Star> dataPackage)
    {
        var constellations = dataPackage.Constellations;
        mesh.ClearSurfaces();
        foreach (var l in ConstellationLabels.GetChildren()) { l.Free(); } 
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

    public void ToggleConstellationLines(object source, bool showlines)
    {
        Visible = showlines;
    }

    public void ToggleConstellationLabels(object source, bool showlabels) { ConstellationLabels.Visible = showlabels; }

    private Star SpawnStar(HorizontalStar horizontalStar)
    {
        Star star = StarScene.Instantiate<Star>();
        star.azimuth = (float)horizontalStar.Azimuth;
        star.altitude = (float)horizontalStar.Altitude;
        star.mag = (float)horizontalStar.Magnitude;
        star.starName = horizontalStar.StarName;

        AddChild(star);
        return star;
    }
}
