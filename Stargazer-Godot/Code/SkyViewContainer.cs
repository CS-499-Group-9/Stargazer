using Godot;
using System;
namespace Stargazer
{
    /// <summary>
    /// The parent container of the <see cref="Stargazer.SkyView"/> object (used for anchoring).
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    public partial class SkyViewContainer : SubViewportContainer
    {
        /// <summary>
        /// Retrieves a reference to the <see cref="Stargazer.SkyView"/>
        /// </summary>
        [Export] public SkyView SkyView { get; set; }


    }
}