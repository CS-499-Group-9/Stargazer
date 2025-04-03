using Godot;
using System;
using System.Collections.Generic;

namespace Stargazer
{

    /// <summary>
    /// Displays the latitude value on the grid.
    /// </summary>
    public partial class GridLabel : Control

    {
        private Camera3D camera;
        private Dictionary<int, Label> gridlabels = new Dictionary<int, Label>();
        private Dictionary<int, Label> azimuthlabels = new Dictionary<int, Label>();

        private float lineInterval;
        private bool render;

        /// <summary>
        /// Instantiates all the grid labels that may be needed.
        /// </summary>
        public override void _Ready()
        {

            lineInterval = 15;
            // Create labels
            for (int altitude = 0; altitude < 180; altitude++)
            {
                Label gridlabel = new Label();
                gridlabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.5f, 0.4f, 0.8f));
                gridlabel.Text = $"{-90 + altitude * lineInterval}°";
                gridlabel.AddThemeFontSizeOverride("font_size", 25);
                gridlabel.SetAnchorsPreset(Control.LayoutPreset.Center);
                AddChild(gridlabel);
                gridlabels[altitude] = gridlabel;
            }
            for (int azimuth = 0; azimuth < 360; azimuth++)
            {
                Label gridlabel = new Label();
                gridlabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.5f, 0.4f, 0.8f));
                gridlabel.Text = $"{azimuth * lineInterval}°";
                gridlabel.AddThemeFontSizeOverride("font_size", 25);
                gridlabel.SetAnchorsPreset(Control.LayoutPreset.Center);
                AddChild(gridlabel);
                azimuthlabels[azimuth] = gridlabel;
            }
        }


        public void HandleZoomStateChanged(ZoomState zoomState)
        {
            switch (zoomState)
            {
                case ZoomState.FullOut:
                    lineInterval = 15.0f;
                    break;
                case ZoomState.Middle:
                    lineInterval = 10.0f;
                    break;
                case ZoomState.FullIn:
                    lineInterval = 1.0f;
                    break;
            }
            updateLabels();
        }

        public void HandleCameraRotationChanged(Camera3D camera)
        {
                const float radians = Mathf.Pi / 180.0f;
                var leftPlane = camera.GetFrustum()[2];
                var plane = leftPlane.Normal;
                var a = plane[0];
                var b = plane[1];
                var c = plane[2];

                var countdraw = 0;
                foreach (var kvp in gridlabels)
                {

                    int index = kvp.Key;
                    Label label = kvp.Value;

                    float altitude = -90 + index * lineInterval;
                    float maxaltdraw = (camera.Rotation.X) * 180.0f / Mathf.Pi + camera.Fov;
                    float minaltdraw = (camera.Rotation.X) * 180.0f / Mathf.Pi - camera.Fov;
                    if (altitude <= 75 && altitude >= -75 && altitude < maxaltdraw && altitude > minaltdraw)
                    {
                        countdraw += 1;
                        label.Visible = true;
                        var tantheta = Mathf.Tan((altitude) * Mathf.Pi / 180.0f);
                        var bigsqrt = MathF.Sqrt(c * c * (a * a - b * b * tantheta * tantheta + c * c));

                        Vector3 placement = new Vector3(100, 0, 0);
                        var flipfactor = 1;
                        if (c < 0)
                        {
                            flipfactor = -1;
                        }
                        if (a * a + c * c != 0 && c != 0)
                        {
                            placement = new Vector3
                            (
                                x: (flipfactor * -1 * bigsqrt - a * b * tantheta) / (a * a + c * c),
                                y: tantheta,
                                z: (flipfactor * a * bigsqrt - b * c * c * tantheta) / (c * (a * a + c * c))
                            );
                        }
                        var unprojected = camera.UnprojectPosition(75f * placement.Normalized());
                        label.Position = new Vector2(0.0f, unprojected[1] - 30.0f);//new Vector2(0,unprojected[1]);
                    }
                    else
                    {
                        label.Visible = false;
                    }
                }

                //Draw Azimuth Labels
                int flip = 1;
                Vector3 nearNormal = -camera.Basis.Z;
                if (nearNormal.Z >= 0)
                {
                    flip = -1;
                }
                var bottomPlane = camera.GetFrustum()[5]; // Bottom plane
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
                foreach (var kvp in azimuthlabels)
                {
                    int index = kvp.Key;
                    Label label = kvp.Value;
                    label.Visible = false;
                    float azimuth = index * lineInterval;
                    if (azimuth >= 360) { break; }
                    if (azimuth != 0)
                    {
                        label.Visible = true;
                        // // Compute t such that tan(theta) = x / z
                        float tanTheta = Mathf.Tan(Mathf.DegToRad(90 - azimuth));

                        float parameterT = (x0 - z0 * tanTheta) / (direction.Z * tanTheta - direction.X);
                        Vector3 placement = 75 * (p0 + direction * parameterT).Normalized();
                        if (flip * placement.Z < 0 && azimuth < 180)
                        {
                            label.Visible = false;
                        }
                        else if (flip * placement.Z > 0 && azimuth > 180)
                        {
                            label.Visible = false;
                        }
                        if (flip * placement.X < 0 && azimuth == 0)
                        {
                            label.Visible = false;
                        }
                        if (flip * placement.X > 0 && azimuth == 180)
                        {
                            label.Visible = false;
                        }
                        Vector2 placement2d = camera.UnprojectPosition(placement);
                        if (azimuth == 180)
                        {
                            var invertedplacement = -placement;
                            azimuthlabels[0].Visible = true;
                            var zero2d = camera.UnprojectPosition(invertedplacement);
                            if (zero2d == placement2d && label.Visible)
                            {
                                azimuthlabels[0].Visible = false;
                            }
                            azimuthlabels[0].Position = new Vector2(zero2d[0], zero2d[1] - 30);
                        }
                        label.Position = new Vector2(placement2d[0], placement2d[1] - 30);
                    }
                }


        }

        private void updateLabels()
        {
            var childlabels = GetChildren();
            var altitude = -90.0;
            var azimuth = 0.0;
            foreach (Label label in gridlabels.Values)
            {
                label.Text = $"{altitude}°";
                altitude += lineInterval;
                label.Visible = false;
            }
            foreach (Label label in azimuthlabels.Values)
            {
                label.Text = $"{azimuth}°";
                azimuth += lineInterval;
                label.Visible = false;

            }
        }



        /// <summary>
        /// Receives the <see cref="ControlContainer.AzimuthToggled"/> notification to show or hide the labels.
        /// </summary>
        /// <param name="showLines"></param>
        public void ToggleGridlines(bool showLines)
        {
            Visible = showLines;

        }
    }
}