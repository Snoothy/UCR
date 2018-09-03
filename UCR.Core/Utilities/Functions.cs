using System;
using System.Diagnostics;

namespace HidWizards.UCR.Core.Utilities
{
    public static class Functions
    {
        /// <summary>
        /// Inverts an axis.
        /// Given Max or Min values, will return the opposite extreme.
        /// Else returns value * -1
        /// </summary>
        /// <param name="value">The raw value of the axis</param>
        /// <returns>The inverted value of the axis</returns>
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

        /// <summary>
        /// Ensures that an axis value is within permitted range
        /// </summary>
        /// <param name="value">The raw axis value</param>
        /// <returns>The clamped axis value</returns>
        public static long ClampAxisRange(long value)
        {
            if (value == 0) return value;
            if (value <= Constants.AxisMinValue) return Constants.AxisMinValue;
            return value >= Constants.AxisMaxValue ? Constants.AxisMaxValue : value;
        }

        /// <summary>
        /// Returns either the low or high half of the axis.
        /// Stretches the half axis returned to fill the full scale
        /// </summary>
        /// <param name="axis">The value of the axis</param>
        /// <param name="positiveRange">Set to true for the high half, else the low half</param>
        /// <returns>The new value for the split axis. If axis is negative and high is specified, returns 0. If axis is positive and low is specified, returns 0</returns>
        public static long SplitAxis(long axis, bool positiveRange)
        {
            long value;
            if (axis == 0) return Constants.AxisMinValue;
            if (positiveRange)
            {
                if (axis < 0) return Constants.AxisMinValue;
                value = axis;
                if (value == Constants.AxisMaxValue) value++;
            }
            else
            {
                if (axis > 0) return Constants.AxisMinValue;
                value = axis * -1;
            }

            value *= 2;
            value += Constants.AxisMinValue;

            if (value == 32768) value = 32767;

            return value;
        }

    }
}
