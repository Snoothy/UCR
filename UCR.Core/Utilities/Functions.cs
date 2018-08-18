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
            // TODO https://github.com/evilC/UCR/blob/master/Libraries/StickOps/StickOps.ahk#L60
            return value;
        }

        public static long ApplyRelativeIncrement(long last, long prev, int sensitivity)
        {
            var sensitivityPercent = (sensitivity / 100.0);
            last = (long)(last * sensitivityPercent);
            return last + prev;
        }

        public static long ApplyContinueRelativeIncrement(long last, long prev, int sensitivity)
        {
            // placeholder!
            var sensitivityPercent = (sensitivity / 100.0);
            last = (long)(last * sensitivityPercent);
            long output = 0;
            output += (last + prev);
            return output;
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

            return invert ? value * -1 : value;
        }
    }
}