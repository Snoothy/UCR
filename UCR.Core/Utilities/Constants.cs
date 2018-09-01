namespace HidWizards.UCR.Core.Utilities
{
    public static class Constants
    {
        public const int MaxDevices = 16;
        public const int AxisMaxValue = short.MaxValue; // ToDo: This will be 32767, meaning that negative scaling does not go to -32768. Fix.
        public const int AxisMinValue = short.MinValue;
    }
}
