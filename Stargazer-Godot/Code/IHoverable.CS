using System;

namespace Stargazer
{
    /// <summary>
    /// Indicates that an object is capable of providing test to display when the mouse hovers over it.
    /// </summary>
    public interface IHoverable
    {
        /// <summary>
        /// Gets the text to be be displayed in the <see cref="HoverLabel"/>
        /// </summary>
        /// <returns>The formatted text <see cref="string"/> to display.</returns>
        public String GetHoverText();
    }
}