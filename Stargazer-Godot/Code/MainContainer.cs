using Godot;

namespace Stargazer
{
    /// <summary>
    /// The main container of the program.
    /// </summary>
    public partial class MainContainer : VBoxContainer
    {

        /// <summary>
        /// Gets a reference to the <see cref="Startup"/> object.
        /// </summary>
        /// <returns>The reference</returns>
        public Startup GetStartup() { return GetNode<Startup>("InteractionContainer"); }
    }
}