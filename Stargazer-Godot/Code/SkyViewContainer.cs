using Godot;

namespace Stargazer
{
	/// <summary>
	/// A basic container to hold the <see cref="Stargazer.SkyView"/>
	/// </summary>
	public partial class SkyViewContainer : SubViewportContainer
	{
		/// <summary>
		/// A reference to the <see cref="Stargazer.SkyView"/> child.
		/// </summary>
		public SkyView SkyView { get; private set; }
		
		/// <summary>
		/// Gets the reference to the <see cref="Stargazer.SkyView"/> child
		/// </summary>
		public override void _Ready()
		{
			base._Ready();
			SkyView = GetNode<SkyView>("SubViewport/SkyView");
		}
	}
}