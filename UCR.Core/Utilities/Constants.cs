namespace HidWizards.UCR.Core.Utilities
{
    public static class Constants
    {
        public const int MaxDevices = 16;
        /// <summary>
        /// When doing calculations using the size of the range from the mid-point to the extreme...
        /// ...use <see cref="AxisMaxAbsValue"/> NOT <see cref="AxisMaxValue"/>.
        /// Signed numbers are larger on the negative side, so perform calculations using the size of the negative scale...
        /// ...and then clamp the value 
        /// </summary>
        public const int AxisMaxAbsValue = short.MinValue * -1; // For short, this should be 32768
        public const int AxisMaxValue = short.MaxValue; // ToDo: This will be 32767, meaning that negative scaling does not go to -32768. Fix.
        public const int AxisMinValue = short.MinValue;
    }
}
