using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidWizards.UCR.Core.Utilities.AxisHelpers
{
	public class ThresholdHelper
	{
		private double _axisMax;
		private double _axisMin;

		/// <summary>
		/// Retrieve Threshold value from plugin.
		/// </summary>
		public int Threshold
		{
			get => _threshold;
			set
			{
				_threshold = value;
				PrecalculateValues();
			}
		}

		private int _threshold;
		
		public ThresholdHelper()
		{
			PrecalculateValues();
		}

		/// <summary>
		/// Calculate maximum and minimum ranges.
		/// </summary>
		private void PrecalculateValues()
		{
			var scaleRange = Threshold / 100d;
			_axisMax = (Constants.AxisMaxValue * scaleRange);
			_axisMin = _axisMax * -1;
		}
		
		/// <summary>
		/// Apply a limit to the maximum values of both positive and negative values.
		/// Threshold is indicated as a percent of the maximum range.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public long LimitAxisRange(long value)
		{
			if (value >= _axisMax)
				value = (long) _axisMax;
			else if (value <= _axisMin)
				value = (long) _axisMin;

			return value;
		}
	}
}