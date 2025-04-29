using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stargazer
{


    /// <summary>
    /// Draws all stars that are not a part of constellations
    /// Author: William Arnett
    /// Created: SPR 2025
    /// Refactored by Josh Johner (SPR 2025) to use IDictionary.
    /// </summary>
    public partial class Spawner2D : Node2D
    {
        private int count = 0;
        private const float radians = (float)Math.PI / 180f;
        /// <summary>
        /// The scene used to instantiate the star objects.
        /// </summary>
        [Export] public PackedScene Star2DScene { get; set; }
        /// <summary>
        /// The scene used to instantiate the labels for stars
        /// </summary>
        [Export] public PackedScene Label2DScene { get; set; }

        private Node2D Star2dContainer;
        private IDictionary<int, Star2D> DrawnStars;

        public override void _Ready()
        {
            DrawnStars = new Dictionary<int, Star2D>();
        }

        /// <summary>
        /// Receives the notification to draw the new star scene.
        /// </summary>
        /// <param name="starDictionary">The <see cref="IDictionary{Int32, Star}"/> that contains the stars to draw.</param>
        public async Task<IDictionary<int, Star2D>> DrawStars(IDictionary<int, Star> starDictionary)
        {
            var oldContainer = Star2dContainer;
            DrawnStars.Clear();
            Star2dContainer = new();

            await Task.Run(() =>
            {
                foreach (var entry in starDictionary)
                {
                    SpawnStar(entry.Value);
                }
            });
            oldContainer?.Free();

            AddChild(Star2dContainer);
            return DrawnStars;
        }


        private Star2D SpawnStar(Star godotStar)
        {
            Star2D outstar = Star2DScene.Instantiate<Star2D>();
            outstar.From3dStar(godotStar);
            Star2dContainer.AddChild(outstar);
            DrawnStars.Add(godotStar.HipparcosId, outstar);
            return outstar;
        }


    }
}