using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;

namespace Stargazer
{
	/// <summary>
	/// A Messier Deep Space Object drawn in the sky.
	/// </summary>
	public partial class MessierObject : CelestialBody
	{

		private HorizontalMessierObject horizontalMessierObject;
		
		/// <summary>
		/// Sets the initial scale of the object.
		/// </summary>
		public override void _Ready()
		{
			Scale = new Vector3(2, 2, 2);
			return;
			var Magnitude = (float)horizontalMessierObject.Magnitude;
			if (Magnitude > 1) Scale = new Vector3(1 / Magnitude, 1 / Magnitude, 1 / Magnitude);
			else Scale = new Vector3(0.6F, 0.6F, 0.6F);
		}

		/// <inheritdoc/>
		public override string GetHoverText()
		{
			return $"{horizontalMessierObject.MessierId}\n + " +
				$"Altitude: {horizontalMessierObject.Altitude}\n" +
				$"Azimuth: {horizontalMessierObject.Azimuth}\n + " +
				$"Distance: {horizontalMessierObject.Distance}\n" +
				$"Size: {horizontalMessierObject.Size} lightyears\n" +
				$"Viewing Season: {horizontalMessierObject.ViewingSeason}";
		}

		/// <summary>
		/// Overrides the <see cref="CelestialBody.FromHorizontal(HorizontalBody, IEquatorialCalculator)"/> 
		/// </summary>
		/// <param name="messierObject">The horizontal object to base this object on.</param>
		/// <param name="calculator">The <see cref="IEquatorialCalculator"/> used to calculate this objects position.</param>
		public void FromHorizontal(HorizontalMessierObject messierObject, IEquatorialCalculator calculator)
		{
			horizontalMessierObject = messierObject;
			base.FromHorizontal(messierObject, calculator);
		}
	}
}