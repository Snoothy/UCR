using System;

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
            const double AxisRange = 1d * (Constants.AxisMaxValue - Constants.AxisMinValue);

            // Map value to -1 .. 1
            var valfx = (((value - Constants.AxisMinValue) / AxisRange) * 2) - 1;

            // calculate (Sensitivity * Value) + ( (1-Sensitivity) * Value^3 )
            var valout = (sens * valfx) + ((1 - sens) * Math.Pow(valfx, 3));

            // Map value back to AxisRange
            return (long)Math.Round(((valout + 1) / 2d) * AxisRange + (1d * Constants.AxisMinValue));
        }

        public static long ApplyCurvedResponse(long value, double curve)
        {
            var curveSens = curve;
            long output = 0;

            // Normalize the output so it always fit in the axis range
            var curveRatio = (Math.Pow(Constants.AxisMaxValue, curveSens)) / Constants.AxisMaxValue;

            // Apply an exponential curve
            if (value > 0)
            {
                output = ((long)((Math.Pow(value, curveSens)) / curveRatio));
            }
            // for negative value, we need to convert them in positive, then in negative again after the
            // exponential calculation
            else if (value < 0)
            {
                value = value * -1;
                value = ((long)((Math.Pow(value, curveSens)) / curveRatio));
                output = value * -1;
            }

            return output;
        }

        public static long ApplyRelativeIncrement(long last, long prev, int sensitivity)
        {
            var sensitivityPercent = (sensitivity / 100.0);
            last = (long)(last * sensitivityPercent);
            return last + prev;
        }

        public static long HalfAxisToFullRange(long axis, bool positiveRange, bool invert)
        {
            long value;
            value = positiveRange ? axis > 0L ? axis : 0L : axis < 0 ? axis * -1 : 0L;

            value = Constants.AxisMinValue + value * 2;

            return invert ? value * -1 : value;
        }
    }
}