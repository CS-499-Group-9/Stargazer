using Godot;
using Stargazer;
using System;
using System.Collections.Generic;

public partial class GridLabel : Control
{
    private Camera3D camera;
    private Dictionary<int, Label> gridlabels = new Dictionary<int, Label>();
    private Dictionary<int, Label> azimuthlabels = new Dictionary<int, Label>();
    private Plane leftPlane;
    private float storedfov;
    private float lineinterval;
    private bool render;
    public override void _Ready()
    {
       
        lineinterval = 15;
        // Create labels
        for (int altitude = 0; altitude < 180; altitude++)
        {
                Label gridlabel = new Label();
                gridlabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.5f, 0.4f, 0.8f));
                gridlabel.Text = $"{-90 + altitude * lineinterval}°";
                gridlabel.AddThemeFontSizeOverride("font_size", 25);
                gridlabel.SetAnchorsPreset(Control.LayoutPreset.Center);
                AddChild(gridlabel);
                gridlabels[altitude] = gridlabel;
        }
        for (int azimuth = 0; azimuth < 360; azimuth++)
        {
                Label gridlabel = new Label();
                gridlabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.5f, 0.4f, 0.8f));
                gridlabel.Text = $"{azimuth * lineinterval}°";
                gridlabel.AddThemeFontSizeOverride("font_size", 25);
                gridlabel.SetAnchorsPreset(Control.LayoutPreset.Center);
                AddChild(gridlabel);
                azimuthlabels[azimuth] = gridlabel;

        }
        
    }
    public override void _Process(double delta)
    {
        if(render){
            leftPlane = camera.GetFrustum()[2];
            const float radians = Mathf.Pi / 180.0f;
            var plane = leftPlane.Normal;
            var a = plane[0];
            var b = plane[1];
            var c = plane[2];
            
            if (camera.Fov < 15 && storedfov >= 15){
                lineinterval = 1.0f;
                updateLabels();
            }
            else if (camera.Fov > 15 && storedfov <= 15){
                lineinterval = 10.0f;
                updateLabels();
            }
            else if (camera.Fov < 35 && storedfov >= 35){
                lineinterval = 10.0f;
                updateLabels();
            }
            else if (camera.Fov > 35 && storedfov <= 35){
                lineinterval = 15.0f;
                updateLabels();
            }
            storedfov = camera.Fov;
            var countdraw = 0;
            foreach (var kvp in gridlabels)
            {
                
                int index = kvp.Key;
                Label label = kvp.Value;

                float altitude = -90 + index * lineinterval;
                float maxaltdraw = (camera.Rotation.X)*180.0f/Mathf.Pi+camera.Fov;
                float minaltdraw = (camera.Rotation.X)*180.0f/Mathf.Pi-camera.Fov;
                if(altitude<= 75 && altitude >= -75 && altitude < maxaltdraw && altitude > minaltdraw){
                    countdraw += 1;
                    label.Visible = true;
                    var tantheta = Mathf.Tan((altitude) * Mathf.Pi/180.0f);
                    var bigsqrt = MathF.Sqrt(c*c*(a*a-b*b*tantheta*tantheta+c*c));

                    Vector3 placement = new Vector3(100,0,0);
                    var flipfactor = 1;
                    if(c < 0){
                        flipfactor = -1;
                    }
                    if(a*a + c*c != 0 && c != 0){
                        placement = new Vector3(
                        x:(flipfactor*-1*bigsqrt-a*b*tantheta)/(a*a+c*c),
                        y:tantheta,
                        z:(flipfactor*a*bigsqrt-b*c*c*tantheta)/(c*(a*a+c*c))
                        );
                    }
                    var unprojected = camera.UnprojectPosition(75f*placement.Normalized());
                    label.Position = new Vector2(0.0f,unprojected[1]-30.0f);//new Vector2(0,unprojected[1]);
                }else{
                    label.Visible = false;
                }

            }

            //Draw Azimuth Labels
            int flip = 1;
            Plane bottomPlane = camera.GetFrustum()[5]; // Bottom plane
            Vector3 nearNormal = -camera.Basis.Z;
            if(nearNormal.Z >= 0){
                flip = -1;
            }

            Vector3 bottomNormal = bottomPlane.Normal;
            // Direction of the intersection line
            Vector3 direction = nearNormal.Cross(bottomNormal);
            // Solve for a point on the line (substituting z = -near)
            float nearDistance = 5;
            float A = bottomNormal.X;
            float B = bottomNormal.Y;
            float C = bottomNormal.Z;
            float D = bottomPlane.D;

            // Solve Ax + By - C * near + D = 0
            float x0 = 0; // Assuming x = 0 for simplicity
            float y0 = (-D - C * -nearDistance) / B; // Solve for y
            float z0 = -nearDistance;
            Vector3 p0 = new Vector3(x0, y0, z0);
            foreach(var kvp in azimuthlabels){
                int index = kvp.Key;
                Label label = kvp.Value;
                label.Visible = false;
                float azimuth = index * lineinterval;
                if (azimuth >= 360){break;}
                if (azimuth != 0){
                    label.Visible = true;
                    // // Compute t such that tan(theta) = x / z
                    float tanTheta = Mathf.Tan(Mathf.DegToRad(90-azimuth));

                    float parameterT = (x0 - z0 * tanTheta) / (direction.Z * tanTheta - direction.X);
                    Vector3 placement = 75*(p0 + direction * parameterT).Normalized();
                    if (flip*placement.Z < 0 && azimuth < 180){
                        label.Visible = false;
                    }else if (flip*placement.Z > 0 && azimuth > 180){
                        label.Visible = false;
                    }
                    if(flip*placement.X < 0 && azimuth == 0){
                        label.Visible = false;
                    }
                    if(flip*placement.X > 0 && azimuth == 180){
                        label.Visible = false;
                    }
                    Vector2 placement2d = camera.UnprojectPosition(placement);
                    if(azimuth == 180){
                        var invertedplacement = -placement;
                        azimuthlabels[0].Visible = true;
                        var zero2d = camera.UnprojectPosition(invertedplacement);
                        if (zero2d == placement2d && label.Visible){
                            azimuthlabels[0].Visible = false;
                        }
                        azimuthlabels[0].Position = new Vector2(zero2d[0],zero2d[1]-30);
                    }
                    label.Position = new Vector2(placement2d[0],placement2d[1]-30);
                }
                

            }
        }else{
            foreach (Label label in gridlabels.Values)
            {
                label.Visible = false;
            }
            foreach (Label label in azimuthlabels.Values)
            {
                label.Visible = false;
            }
        }

    }

    public void SetCamera(Camera3D camera)
    {
        this.camera = camera;
        storedfov = camera.Fov;
    }
    private void updateLabels(){
        var childlabels = GetChildren();
        var altitude = -90.0;
        var azimuth = 0.0;
        foreach (Label label in gridlabels.Values){
            label.Text = $"{altitude}°";
            altitude += lineinterval;
            label.Visible = false;
        }
        foreach (Label label in azimuthlabels.Values){
            label.Text = $"{azimuth}°";
            azimuth += lineinterval;
            label.Visible = false;
        }
    }
    public void ToggleGridlines(bool showLines)
    {

        render = showLines;
    }

}
