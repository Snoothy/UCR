using System;
using System.Diagnostics;

namespace HidWizards.UCR.Core.Utilities
{
    public static class Functions
    {
        public static long ApplyRangeDeadZone(long value, int deadZonePercentage)
        {
            var gap = (deadZonePercentage / 100.0) * Constants.AxisMaxValue;
            var remainder = Constants.AxisMaxValue - gap;
            var gapPercent = Math.Max(0, Math.Abs(value) - gap) / remainder;
            return (long)(gapPercent * Constants.AxisMaxValue * Math.Sign(value));
        }

        public static long ApplyRangeSensitivity(long value, int sensitivity, bool linear)
        {
            var sensitivityPercent = (sensitivity / 100.0);
            if (linear) return (long)(value * sensitivityPercent);

            var sens = sensitivityPercent / 100d;
            double AxisRange = 1d * (Constants.AxisMaxValue - Constants.AxisMinValue);
            // Map value to -1 .. 1
            double val11 = (((value - Constants.AxisMinValue) / AxisRange) * 2) - 1;
            // calculate (Sensitivity * Value) + ( (1-Sensitivity) * Value^3 )
            double valout = (sens * val11) + ((1 - sens) * Math.Pow( val11, 3 ));
            // Map value back to AxisRange
            value = (long) Math.Round( ((valout + 1) / 2d) * AxisRange + (1d * Constants.AxisMinValue) );

            return value;
        }

        public static long HalfAxisToFullRange(long axis, bool positiveRange, bool invert)
        {
            long value;
            if (positiveRange)
            {
                value = axis > 0L ? axis : 0L;
            }
            else
            {
                value = axis < 0 ? axis * -1 : 0L;
            }

            value = Constants.AxisMinValue + value * 2;
            value = invert ? value * -1 : value;
            value = Math.Min(Math.Max(value, Constants.AxisMinValue), Constants.AxisMaxValue);
            return value; 
        }
    }
}
