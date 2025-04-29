using Godot;

namespace Stargazer
{
    /// <summary>
    /// Handles the transparency of the floor.
    /// Author: Logan Parker
    /// Created: SPR 2025
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
            mesh = GetNode<MeshInstance3D>("MeshInstance3D"); // Fetch the child MeshInstance3D
            material = mesh.GetActiveMaterial(0) as StandardMaterial3D;
            material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha; // Ensure transparency is enabled   
        }

        /// <summary>
        /// Subscribes to the camera OnRotation notification. 
        /// Author: Josh Johner
        /// Created: SPR 2025
        /// </summary>
        /// <param name="camera"></param>
        public void HandleCameraRotationChanged(Camera3D camera)
        {
            float xRotation = camera.Rotation.X; // Get rotation in radians
            float alpha = Mathf.Clamp(0.5f + xRotation, 0.1f, 1f);

            Color albedoColor = material.AlbedoColor;
            albedoColor.A = alpha; // Apply transparency
            material.AlbedoColor = albedoColor;
        }
    }
}