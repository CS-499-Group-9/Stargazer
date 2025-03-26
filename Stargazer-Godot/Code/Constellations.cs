using DataLayer;
using DataLayer.EquatorialObjects;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stargazer
{

	/// <summary>
	/// The <see cref="Node3D"/> used to contain the constellation stars and lines in the viewport.
	/// </summary>
	public partial class Constellations : Node3D
	{
		/// <summary>
		/// The scene used to instantiate the stars in the constellation
		/// </summary>
		[Export] public PackedScene StarScene { get; set; }
		/// <summary>
		/// The scene used to instantiate the labels in the constellation
		/// </summary>
		[Export] public PackedScene LabelScene { get; set; }


		private Node3D? StarContainer;
		private MeshInstance3D? constMesh;
		private Node3D? ConstellationLabels;
		//private List<(Star,Star)> starRefList;
		private IEnumerable<Constellation> constellations; 
		private Func<int, Func<HorizontalStar, Star>, Star> GetConstellationStar;
		private bool canProcess = false;
		private IDictionary<string, LabelNode> labelDictionary;

		public override void _Process(double delta)
		{
			if (!constMesh?.Visible?? true) return;
			
			var mesh = new ImmediateMesh();
 
			mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, constMesh.MaterialOverride);
				
			foreach (var constellation in constellations)
			{
				// Used to aggregate the positions of the stars in the constellation
				Vector3 totalPos = new(0, 0, 0);
				// The number of stars in the constellation
				int c = 0;
				foreach (var lines in constellation.ConstellationLines)
				{
					// Get the stars from the dictionary
					Star s1 = GetConstellationStar(lines.Item1, (s) => { return null; });
					Star s2 = GetConstellationStar(lines.Item2, (s) => { return null; });
					// Draw the line between the stars
					mesh.SurfaceAddVertex(s1.Position3D);
					mesh.SurfaceAddVertex(s2.Position3D);
					if (ConstellationLabels.Visible)
					{
						if (totalPos == Vector3.Zero) // solely checked for the first star
						{
							totalPos += s1.Position;
							c++;
						}

						totalPos += s2.Position;
						c++;
					}
				}
				if (ConstellationLabels.Visible) 
				{ 
					var labelPos = totalPos / c;
					if (labelDictionary.TryGetValue(constellation.ConstellationId, out var label)) label.Position = labelPos; 
				}
			}
			mesh.SurfaceEnd();
			constMesh.Mesh = mesh;
		}

        /// <summary>
        /// Draws the stars and constellation lines for each of the <see cref="Constellation"/>s
        /// </summary>
        /// <param name="constellations">The <see cref="IEnumerable{Constellation}"/> list of constellations</param>
        /// <param name="GetStar">The method used to retrieve a <see cref="Star"/>From the dictionary of drawn stars.</param>
        /// <param name="SpawnStar">The method used to drawn a new star in the <see cref="SkyView"/> <see cref="Star"/> container.</param>
        public async Task DrawConstellations(IEnumerable<Constellation> constellations, Func<int, Func<HorizontalStar, Star>, Star> GetStar, Func<HorizontalStar, Star> SpawnStar)
		{
			canProcess = false;
			this.GetConstellationStar = GetStar;
			this.constellations = constellations;
			labelDictionary = new Dictionary<string, LabelNode>();
			// Get references to the current containers of constellation objects
			var oldMesh = constMesh;
			var oldStars = StarContainer;

			// Create the material used to draw the lines
			StandardMaterial3D whiteMaterial = new()
			{

				//GD.Print($"Drawing constellation {constellation.ConstellationName}");
				AlbedoColor = new Color(0.8f, 0.8f, 0.8f, 0.8f), // White color
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
			};
			Vector3 totalPos = new Vector3(0, 0, 0);
			int c = 0;
			// Create new containers for stars, labels and constellation lines
			StarContainer = new();
			ConstellationLabels = new();
			constMesh = new()
			{
				MaterialOverride = whiteMaterial
			};

			// Create a new mesh for the constellation lines
			var mesh = new ImmediateMesh();

			

			// Create a new task to complete all the calculations and await that task to complete
			await Task.Run(() =>
			{

				mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, constMesh.MaterialOverride);

				foreach (var constellation in this.constellations)
				{
					
					// Used to aggregate the positions of the stars in the constellation
					Vector3 totalPos = new(0, 0, 0);
					// The number of stars in the constellation
					int c = 0;
					// int linecount = 0;
					// foreach (var lines in constellation.ConstellationLines){
					// 	linecount += 1;
					// }
					
					// int lineindex = 0;
					foreach (var lines in constellation.ConstellationLines)
					{
						// Get the stars from the dictionary
						Star s1 = GetStar(lines.Item1, SpawnStar);
						Star s2 = GetStar(lines.Item2, SpawnStar);
						// Draw the line between the stars
						mesh.SurfaceAddVertex(s1.Position3D);
						mesh.SurfaceAddVertex(s2.Position3D);

						if (totalPos == Vector3.Zero) // solely checked for the first star
						{
							totalPos += s1.Position3D;
							c++;
						}

						totalPos += s2.Position3D;
						c++;
					}

					// Creating labels
					var labelPos = totalPos / c;

					LabelNode labelNode = LabelScene.Instantiate<LabelNode>();
					labelNode.LabelText = constellation.ConstellationName;
					labelNode.Position = labelPos;
					labelNode.Visible = true;
					labelDictionary.Add(constellation.ConstellationId, labelNode);
					ConstellationLabels.AddChild(labelNode);
				}
				
			});
			mesh.SurfaceEnd();
			constMesh.Mesh  = mesh;

			// If the old containers exist, remove them from the tree
			// oldMesh?.Free();
			// oldStars?.Free();

			foreach(var child in GetChildren()){
				child.Free();
			}
			// Add the new containers to the tree
			AddChild(StarContainer);
			constMesh.AddChild(ConstellationLabels);
			AddChild(constMesh);

			canProcess = true;
		}
		
		/// <summary>
		/// Receives the notification to toggle the visibility of the constellation lines.
		/// Hiding the lines will also hide the labels.
		/// </summary>
		/// <param name="showLines">True if the user has requested to show the lines.</param>
		public void ToggleConstellationLines(bool showLines)
		{
			if (constMesh == null) return;
			constMesh.Visible = showLines;
		}

		/// <summary>
		/// Receives the notification to toggle the visibility of the constellation labels.
		/// </summary>
		/// <param name="showLabels">True if the user has requested to show the labels.</param>
		public void ToggleConstellationLabels(bool showLabels) 
		{
			if (ConstellationLabels == null) return;	
			ConstellationLabels.Visible = showLabels; 
		}

		
	}
}
