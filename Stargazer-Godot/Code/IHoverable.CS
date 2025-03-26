using System;

namespace Stargazer
{
    /// <summary>
    /// Provides the ability of an object to display a hover label.
    /// </summary>
    public interface IHoverable
    {
        /// <summary>
        ///  Gets the text to display.
        /// </summary>
        /// <returns>A formatted text string.</returns>
        public string GetHoverText();
    }
}