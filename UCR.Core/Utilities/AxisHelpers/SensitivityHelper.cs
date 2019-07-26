using System;

namespace HidWizards.UCR.Core.Utilities.AxisHelpers
{
    public class SensitivityHelper
    {
        private double _scaleFactor;
        private double _axisRange;
        private double _sens;

        public int Percentage
        {
            get => _percentage;
            set
            {
                // Do NOT throw if percentage is not in range 0..100, other values are valid!
                _percentage = value;
                PrecalculateValues();
            }
        }

        private int _percentage;

        public bool IsLinear { get; set; }

        public SensitivityHelper()
        {
            PrecalculateValues();
        }

        private void PrecalculateValues()
        {
            _scaleFactor = _percentage / 100d;
            _axisRange = Constants.AxisMaxValue - Constants.AxisMinValue;
            _sens = _scaleFactor / 100d;
        }

        public short ApplyRangeSensitivity(short value)
        {
            if (IsLinear) return Functions.ClampAxisRange((int) Math.Round(value * _scaleFactor));

            // Map value to -1 .. 1
            double val11 = (((value - Constants.AxisMinValue) / _axisRange) * 2d) - 1d;
            // calculate (Sensitivity * Value) + ( (1-Sensitivity) * Value^3 )
            double valout = (_sens * val11) + ((1d - _sens) * Math.Pow(val11, 3d));
            // Map value back to AxisRange
            value = (short) Math.Round(((valout + 1d) / 2d) * _axisRange + (1d * Constants.AxisMinValue));

            return value;
        }

    }
}
