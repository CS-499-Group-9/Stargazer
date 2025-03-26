using DataLayer.HorizontalObjects;
using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Stargazer
{
	/// <summary>
	/// A star that has been converted from Horizontal Coordinate form, into Godot coordinate form and drawn to the screen.
	/// </summary>
	public partial class Star2D : Node2D
	{

		private const float radians = (float)Math.PI / 180f;

		// Since these are not being connected to anything in the Godot interface, I'm not sure we need to use the Export attribute.
		// They are all accessed/set via code.
		// Not sure what overhead is involved in labeling these as export.

        private Star? star3d;
        public float Altitude { get { return (float)star3d?.Altitude; } }
        public float Azimuth { get { return (float)star3d?.Azimuth; } }

        public string StarName { get { return star3d?.StarName; } }

        private Vector2 getLocation()
        {

            Vector2 Pos2D = new Vector2(
                x:(90.0f-Altitude)*Mathf.Cos((-Azimuth-90)*radians),
                y:(90.0f-Altitude)*Mathf.Sin((-Azimuth-90)*radians)
            );
            return Pos2D;
        }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
            ColorRect colorRect = GetNode<ColorRect>("ColorRect");
            if (Altitude < 0.5){
                Visible = true;
            }
			Position = getLocation();
            ShaderMaterial shaderMaterial = (ShaderMaterial)GetMaterial();
            if (colorRect != null && colorRect.Material is ShaderMaterial mat){
                shaderMaterial = (ShaderMaterial)mat.Duplicate();
                // if(!string.IsNullOrWhiteSpace(starName)){
                //     //GD.Print($"I found it.\n{starName}");
                //     shaderMaterial.SetShaderParameter("starColor",new Vector3(1.0F,0.0F,0.0F));
                // }
                //shaderMaterial.SetShaderParameter("starColor",new Vector3(Mathf.Floor(altitude/15)/7.0F,Mathf.Floor(altitude/15)/7.0F,0.5F));  
                colorRect.Material = shaderMaterial;
            }

            //Position = new Vector2(0.0F,0.0F);
			// if (mag > 1) Scale = new Vector2(1 / mag, 1 / mag);
			// else Scale = new Vector2(0.6F, 0.6F);
            Scale = new Vector2(0.3F,0.3F);

		}
        public void scalestar(Vector2 scale){
            Scale = scale;
        }

        public void From3dStar(Star star)
        {
            star3d = star;
        }

	}
}