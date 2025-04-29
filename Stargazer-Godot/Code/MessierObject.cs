using DataLayer.HorizontalObjects;
using DataLayer.Interfaces;
using Godot;
using Stargazer;
using System;

namespace Stargazer
{
	/// <summary>
	/// A Messier Deep Space Object drawn in the sky.
	/// Author: Josh Johner
	/// Created: SPR 2025
	/// Refactored by William Arnett (SPR 2025) to implement hover text.
	/// </summary>
	public partial class MessierObject : CelestialBody
	{

		private HorizontalMessierObject horizontalMessierObject;
		private int zDepth;
		
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
			string CommonName = "";
			if(horizontalMessierObject.CommonName.Length != 0 ){
				CommonName = $"{horizontalMessierObject.CommonName}\n";
			}
			string distanceText = $"{horizontalMessierObject.Distance:0.00} ly";
			if(horizontalMessierObject.Distance >= 1000){
				distanceText = $"{horizontalMessierObject.Distance/1000:0.00} kly";
			}
			if(horizontalMessierObject.Distance >= 1000000){
				distanceText = $"{horizontalMessierObject.Distance/1000000:0.00} Mly";
			}
			return $"{horizontalMessierObject.MessierId}\n" +
				CommonName +
				$"Altitude: {horizontalMessierObject.Altitude:0.00}°\n" +
				$"Azimuth: {horizontalMessierObject.Azimuth:0.00}°\n" +
				$"Distance: "+distanceText+"\n" +
				$"Size: {horizontalMessierObject.Size}\n" +
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
			GetNode<Sprite3D>("MessierBody/Sprite3D").SortingOffset = Int32.Parse(messierObject.MessierId.Trim('M'));
			base.FromHorizontal(messierObject, calculator);
		}
	}
}