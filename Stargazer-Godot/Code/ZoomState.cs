namespace Stargazer
{
    /// <summary>
    /// Used to implement quantum logic in the view based on Fov thresholds.
    /// Author: Josh Johner
    /// Created: SPR 2025
    /// </summary>
    public enum ZoomState
    {
        /// <summary>
        /// The viewport Fov is in the full zoomed out region.
        /// </summary>
        FullOut,
        /// <summary>
        /// The viewport Fov is in the middle zoom region.
        /// </summary>
        Middle,
        /// <summary>
        /// The viewport Fov is in the fully zoomed in region.
        /// </summary>
        FullIn,
    }
}
