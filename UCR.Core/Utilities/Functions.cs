using System;
using System.Diagnostics;

namespace HidWizards.UCR.Core.Utilities
{
    public static class Functions
    {
        public static long Invert(long value)
        {
            if (value == 0) return 0;
            if (value >= Constants.AxisMaxValue)
            {
                return Constants.AxisMinValue;
            }

            if (value <= Constants.AxisMinValue)
            {
                return Constants.AxisMaxValue;
            }

            return value * -1;
        }

        public static long ClampAxisRange(long value)
        {
            if (value == 0) return value;
            if (value <= Constants.AxisMinValue) return Constants.AxisMinValue;
            return value >= Constants.AxisMaxValue ? Constants.AxisMaxValue : value;
        }

        public static long SplitAxis(long axis, bool positiveRange)
        {
            long value;
            if (positiveRange)
            {
                if (axis < 0) return 0;
                value = axis;
                if (value == Constants.AxisMaxValue) value++;
            }
            else
            {
                if (axis > 0) return 0;
                value = axis * -1;
            }

            value *= 2;
            value += Constants.AxisMinValue;

            if (value == 32768) value = 32767;

            return value;
        }

    }
}
