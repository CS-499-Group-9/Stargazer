using DataLayer;
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

public partial class Constellations : Node3D
{
    [Export] public PackedScene StarScene { get; set; }
    [Export] public PackedScene LabelScene { get; set; }

    private void DrawConstellations(object source, CelestialDataPackage<Star> dataPackage)
    {
        var constellations = dataPackage.Constellations;
        MeshInstance3D constMesh = new MeshInstance3D();
        Vector3 labelPos = new Vector3();
        ImmediateMesh mesh = new ImmediateMesh();
        // Create a white material
        StandardMaterial3D whiteMaterial = new StandardMaterial3D();
        whiteMaterial.AlbedoColor = new Color(0.8f, 0.8f, 0.8f, 0.8f); // White color
        whiteMaterial.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        // Assign the material to the mesh
        constMesh.MaterialOverride = whiteMaterial;
        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

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
            labelNode.Visible = labelDrawn;
            labels.Add(labelNode);
        }
        mesh.SurfaceEnd();
        constMesh.Mesh = mesh;
        AddChild(constMesh);
        labels.ForEach((label) => { AddChild(label); });

    }

}
