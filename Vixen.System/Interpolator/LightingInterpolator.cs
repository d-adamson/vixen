﻿using System.Drawing;
using Vixen.Sys;

namespace Vixen.Interpolator {
	[Vixen.Sys.Attribute.Interpolator(typeof(LightingValue))]
	class LightingInterpolator : Interpolator<LightingValue> {
		private ColorInterpolator _colorInterpolator;
		private DoubleInterpolator _doubleInterpolator;

		public LightingInterpolator() {
			_colorInterpolator = new ColorInterpolator();
			_doubleInterpolator = new DoubleInterpolator();
		}

		protected override LightingValue InterpolateValue(double percent, LightingValue startValue, LightingValue endValue) {
			Color newColor;
			double newIntensity;
			
			_colorInterpolator.Interpolate(percent, startValue.Color, endValue.Color, out newColor);
			_doubleInterpolator.Interpolate(percent, startValue.Intensity, endValue.Intensity, out newIntensity);
			
			return new LightingValue(newColor, newIntensity);
		}
	}
}