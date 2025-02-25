using Godot;
using System;

public partial class StarMesh : MeshInstance3D
{

    public override void _Ready()
    {
        // Get the parent (assuming it's a Star Node3D)
        if (GetParent() is Star parent && parent.ColorIndex != null)
        {
            UpdateColorFromParent(parent.ColorIndex);
        }
        else
        {
            GD.PrintErr("Parent does not have a ColorIndex property!");
        }
    }

    private void UpdateColorFromParent(double bvIndex)
    {
        Color starColor = BvToGodotColor(bvIndex);
        ApplyEmissionColor(starColor);
    }

    private Color BvToGodotColor(double bv)
    {
        bv = Math.Clamp(bv, -0.4, 2.0);
        double r, g, b;

        if (bv < 0.0) { r = 0.67 + (bv * 0.34); g = 0.83 + (bv * 0.17); b = 1.00; }
        else if (bv < 0.4) { r = 1.00; g = 0.98 - (bv * 0.16); b = 1.00 - (bv * 0.47); }
        else if (bv < 1.5) { r = 1.00; g = 0.82 - (bv * 0.44); b = 0.55 - (bv * 0.10); }
        else { r = 1.00; g = 0.60 - ((bv - 1.5) * 0.15); b = 0.40 - ((bv - 1.5) * 0.20); }

        return new Color((float)r, (float)g, (float)b);
    }

    private void ApplyEmissionColor(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.EmissionEnabled = true;
        material.Emission = color;
        MaterialOverride = material;
    }
}
