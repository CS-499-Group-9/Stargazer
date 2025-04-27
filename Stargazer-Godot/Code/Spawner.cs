using DataLayer;
using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stargazer
{

    /// <summary>
    /// Draws all stars that are not a part of constellations
    /// Author: Logan Parker
    /// Created: SPR 2025
    /// Refactored by Josh Johner (SPR 2025) for concurrency and single responsibility.
    /// </summary>
    public partial class Spawner : Node3D
    {
        /// <summary>
        /// The scene used to instantiate the star objects.
        /// </summary>
        [Export] public PackedScene StarScene { get; set; }
        /// <summary>
        /// The scene used to instantiate the labels for stars
        /// </summary>
        [Export] public PackedScene LabelScene { get; set; }

        private Node3D StarContainer;
        private IEquatorialCalculator starConverter;
        private BlockingCollection<ConcurrentBag<HorizontalStar>> starProducer;
        const float maxStarMagnitude = 6.0f;
        /// <summary>
        /// Receives the notification to update the stars drawn.
        /// Original Author: Logan Parker (SPR 2025)
        /// Refactored by Josh Johner (SPR 2025) for asynchronization. Func{HorizontalStar,Star} for single responsibility (pass method by reference).
        /// </summary>
        /// <param name="stars">The <see cref="IEnumerable{HorizontalStar}"/> that contains the stars to draw.</param>
        /// <param name="GetStar">Used to retrieve a star from the <see cref="CelestialDataPackage{Star}"/></param>
        /// <param name="starConverter">Used to calculate star data.</param>
        /// 

        public async Task DrawStars(IEnumerable<HorizontalStar> stars, Func<int, Func<HorizontalStar, Star>, Star> GetStar, IEquatorialCalculator starConverter)
        {
            this.starConverter = starConverter;
            // Get a reference to the current star container
            var oldContainer = StarContainer;
            // Create a new star container in memory
            StarContainer = new();

            // Create a new task to calculate the positions of the stars and add them to the container and await completion
            await Task.Run(() =>
            {
                foreach (var star in stars.Where((s) => s.Magnitude < maxStarMagnitude))
                {
                    GetStar(star?.HipparcosId ?? 0, SpawnStar);
                }
            });

            // If the previous container exists, remove it from the tree then add the new container.
            oldContainer?.Free();
            AddChild(StarContainer);
        }

        /// <summary>
        /// Used to spawn a new star in the sky view scene.
        /// I'm rather proud of how this is handled in other classes.
        /// Original Author: Logan Parker (SPR 2025)
        /// Refactored by Josh Johner (SPR 2025) to be passed by reference to other methods to hand concurrent drawing (competing spawners). 
        /// </summary>
        /// <param name="horizontalStar">The <see cref="HorizontalStar"/> to base the <see cref="Star"/> on.</param>
        /// <returns></returns>
        public Star SpawnStar(HorizontalStar horizontalStar)
        {
            Star star = StarScene.Instantiate<Star>();
            star.FromHorizontal(horizontalStar, starConverter);
            if (StarContainer.IsInsideTree())
            {
                // The constellation may need to draw a star to the sky that is not already there (due to the magnitude filter).
                // Since this happens asynchronously, it cannot interact with the scene tree.
                StarContainer.CallDeferred("add_child", star);
            }
            else
            {
                StarContainer.AddChild(star);
            }
            return star;
        }
    }
}
