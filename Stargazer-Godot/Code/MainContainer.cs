using Godot;
using Stargazer;
using System;

namespace Stargazer
{
	/// <summary>
	/// The main container of the entire GUI
	/// </summary>
	public partial class MainContainer : VBoxContainer
	{
		/// <summary>
		/// Used to get the <see cref="Startup"/> object
		/// </summary>
		/// <returns>The <see cref="Startup"/> class atttached to the interaction container.</returns>
		public Startup GetStartup() { return GetNode<Startup>("InteractionContainer"); }
	}
}