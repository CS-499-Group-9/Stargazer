using Godot;
using Stargazer;
using System;
using System.Collections.Generic;

public partial class TestSpheres : Node
{
    private Camera3D camera;
    private SubViewport viewport;
    private Dictionary<int, MeshInstance3D> labels = new Dictionary<int, MeshInstance3D>();
    private Plane leftPlane;
    private float storedfov;
    private float lineinterval;
	private bool update;
    public override void _Ready()
    {
		update = true;
        camera = GetNode<Camera3D>("/root/Control/SubViewportContainer/SubViewport/View/Camera3D");
        viewport = GetNode<SubViewport>("/root/Control/SubViewportContainer/SubViewport");
        storedfov = camera.Fov;
        lineinterval = 15;
        // Create labels
        for (int altitude = 0; altitude < 40; altitude++)
        {
                MeshInstance3D label = GetNode<MeshInstance3D>("MeshInstance3D");
				MeshInstance3D newMesh = new MeshInstance3D();
				newMesh.Mesh = label.Mesh;
                //label.Text = $"{-90 + altitude * lineinterval}°";
                //label.AddThemeFontSizeOverride("font_size", 25);
                //label.SetAnchorsPreset(Control.LayoutPreset.Center);
                AddChild(newMesh);
                labels[altitude] = newMesh;

        }
    }

    public override void _Process(double delta)
    {
		if(update){
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
        foreach (var kvp in labels)
        {
            
            int index = kvp.Key;
            MeshInstance3D label = kvp.Value;

            float altitude = -90 + index * lineinterval;
            float maxaltdraw = (camera.Rotation.X)*180.0f/Mathf.Pi+camera.Fov;
            float minaltdraw = (camera.Rotation.X)*180.0f/Mathf.Pi-camera.Fov;
            //GD.Print($"{camera.Rotation.X*180.0f/Mathf.Pi} + {camera.Fov} = {maxaltdraw}");
            if(altitude<= 90 && altitude >= -90 && altitude < maxaltdraw && altitude > minaltdraw){
                countdraw += 1;
                label.Visible = true;
                var tantheta = Mathf.Tan((altitude) * Mathf.Pi/180.0f);
                var bigsqrt = MathF.Sqrt(Math.Abs(c*c*(a*a-b*b*tantheta*tantheta+c*c)));

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
				// if (placement.IsFinite()){
                label.Position = 50*placement.Normalized();
				// }
				if(altitude == -61 || altitude == -60 || altitude == -59){
					//GD.Print($"{altitude} {placement}");
					//GD.Print(bigsqrt);
				}
				//new Vector2(0,unprojected[1]);
            }else{
                label.Visible = false;
            }
		}

        }
        //GD.Print($"i drew {countdraw}");
    }
	public override void _Input(InputEvent @event)
	{
		if(@event.IsAction("stop")){
			update = false;
		}
	}
    private void updateLabels(){
        var childlabels = GetChildren();
        var altitude = -90.0;
        foreach (MeshInstance3D label in childlabels){
            // label.Text = $"{altitude}°";
            altitude += lineinterval;
        }
        }
}
