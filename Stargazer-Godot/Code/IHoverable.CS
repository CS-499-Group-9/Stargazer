using Godot;

namespace Stargazer
{
    /// <summary>
    /// Provides the ability of an object to display a hover label.
    /// </summary>
    public interface IHoverable : ITrackable
    {
        /// <summary>
        ///  Gets the text to display.
        /// </summary>
        /// <returns>A formatted text string.</returns>
        public string GetHoverText();
      
        
    }
}
