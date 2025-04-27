using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;

namespace Stargazer
{
	/// <summary>
	/// The actual sun.
	/// Author: Josh Johner
	/// Created: SPR 2025
	/// Refactored by William Arnett (SPR 2025) to handle rotation, and hover text. 
	/// </summary>
	public partial class Sun : CelestialBody
	{
		private HorizontalSun sun;

		/// <summary>
		/// Set's the scale (size) of the sun.
		/// </summary>
		public override void _Ready()
		{
			Scale = new Vector3(2, 2, 2);
		}

		/// <summary>
		/// Overrides the <see cref="CelestialBody._Process(double)"/> method since the sun requires specific calculations.
		/// </summary>
		/// <param name="delta"></param>
		public override void _Process(double delta)
		{
			calculator?.UpdatePositionOf(sun);
			DrawnDistance = Distance;
			Position = GetLocation();
			Rotate(Vector3.Up, Mathf.Pi);
			RotationDegrees = new Vector3(0, 0, -90 + 34.7304f);
		}

		/// <inheritdoc/>
		public override string GetHoverText()
		{
			return $"The Sun\n" +
				$"Altitude: {sun.Altitude}\n" +
				$"Azimuth: {sun.Azimuth}\n" +
				$"Distance: {sun.Distance} AU";
		}

		/// <summary>
		/// Overload the <see cref="CelestialBody.FromHorizontal(HorizontalBody, IEquatorialCalculator)"/> method to get a reference to the <see cref="HorizontalSun"/> object.
		/// </summary>
		/// <param name="sun">The horizontal object</param>
		/// <param name="calculator">The <see cref="IEquatorialCalculator"/> used to perform calculations.</param>
		public void FromHorizontal(HorizontalSun sun, IEquatorialCalculator calculator)
		{
			base.FromHorizontal(sun, calculator);
			DrawnDistance = Distance;
			this.sun = sun;
			this.calculator = calculator;
		}
	}
}