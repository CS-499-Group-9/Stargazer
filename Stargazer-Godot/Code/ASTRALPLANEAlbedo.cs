using Godot;

namespace Stargazer
{
    /// <summary>
    /// Handles the transparency of the floor.
    /// </summary>
    public partial class ASTRALPLANEAlbedo : Node3D
    {
        private Camera3D camera;
        private MeshInstance3D mesh;
        private StandardMaterial3D material;

        /// <summary>
        /// Gets references to the <see cref="Camera3D"/> and <see cref="StandardMaterial3D"/> of the <see cref="MeshInstance3D"/>
        /// </summary>
        public override void _Ready()
        {
            camera = (Camera3D)GetTree().Root.FindChild("Camera3D", true, false);
            if (camera == null)
            {
                GD.PrintErr("Camera3D not found!");
                return;
            }

            mesh = GetNode<MeshInstance3D>("MeshInstance3D"); // Fetch the child MeshInstance3D
            if (mesh == null)
            {
                GD.PrintErr("MeshInstance3D not found!");
                return;
            }

            material = mesh.GetActiveMaterial(0) as StandardMaterial3D;
            if (material == null)
            {
                GD.PrintErr("StandardMaterial3D is null! Ensure the material is a StandardMaterial3D.");
            }
            else
            {
                GD.Print("StandardMaterial3D found.");
                material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha; // Ensure transparency is enabled
            }
        }

        /// <summary>
        /// Uses the <see cref="Camera3D"/><c>.Rotation</c> to alter make the floor transparent as the user pans down.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            if (camera == null || material == null)
                return;

            float xRotation = camera.Rotation.X; // Get rotation in radians
            float alpha = Mathf.Clamp(0.5f + xRotation, 0.1f, 1f);

            Color albedoColor = material.AlbedoColor;
            albedoColor.A = alpha; // Apply transparency
            material.AlbedoColor = albedoColor;
        }
    }
}