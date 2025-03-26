using Godot;
using Stargazer;
using System;
using System.Collections.Generic;

public partial class GridLabel : Control
{
    private Camera3D camera;
    private Dictionary<int, Label> gridlabels = new Dictionary<int, Label>();
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
                gridlabel.Text = $"{-90 + altitude * lineinterval}°";
                gridlabel.AddThemeFontSizeOverride("font_size", 25);
                gridlabel.SetAnchorsPreset(Control.LayoutPreset.Center);
                AddChild(gridlabel);
                gridlabels[altitude] = gridlabel;

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
                //GD.Print($"{camera.Rotation.X*180.0f/Mathf.Pi} + {camera.Fov} = {maxaltdraw}");
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
        }else{
            foreach (var kvp in gridlabels)
            {
                
                int index = kvp.Key;
                Label label = kvp.Value;
                label.Visible = false;
            }
        }

        //GD.Print($"i drew {countdraw}");
    }

    public void SetCamera(Camera3D camera)
    {
        this.camera = camera;
        storedfov = camera.Fov;
    }
    private void updateLabels(){
        var childlabels = GetChildren();
        var altitude = -90.0;
        foreach (Label label in childlabels){
            label.Text = $"{altitude}°";
            altitude += lineinterval;
        }
    }
    public void ToggleGridlines(bool showLines)
    {

        render = showLines;
    }

}
