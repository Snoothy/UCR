namespace HidWizards.UCR.Core.Utilities
{
    public static class Constants
    {
        public const int MaxDevices = 16;
        /// <summary>
        /// <para>When doing calculations using the size of the range from the mid-point to the extreme,
        /// use <see cref="AxisMaxAbsValue"/> NOT <see cref="AxisMaxValue"/></para>
        /// <para>Signed numbers are larger on the negative side, so perform calculations using the size of the negative scale,
        /// and then clamp the value</para>
        /// </summary>
        public const int AxisMaxAbsValue = short.MinValue * -1; // For short, this should be 32768
        public const int AxisMaxValue = short.MaxValue;
        public const int AxisMinValue = short.MinValue;
    }
}
