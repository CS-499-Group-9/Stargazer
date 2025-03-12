using Godot;
using System;

namespace Stargazer
{
	/// <summary>
	/// Contains the button used to toggle the constellations in the viewport.
	/// </summary>
	public partial class ConstellationButton : Control
	{
		/// <summary>
		/// The <see cref="Delegate"/> used to notify the viewport to show or hide the constellations.
		/// </summary>
		public Action<bool> ConstellationLinesToggled;

		private void ToggleConst(bool state)
		{
			ConstellationLinesToggled(state);
		}
	}
}