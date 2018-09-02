using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                _percentage = value;
                PrecalculateValues();
            }
        }

        private int _percentage;

        public bool IsLinear
        {
            get => _isLinear;
            set
            {
                _isLinear = value;
                PrecalculateValues();
            }
        }
        
        private bool _isLinear;

        private void PrecalculateValues()
        {
            _scaleFactor = _percentage / 100.0;
            _axisRange = 1d * (Constants.AxisMaxValue - Constants.AxisMinValue);
            _sens = _scaleFactor / 100d;
        }

        public long ApplyRangeSensitivity(long value, int sensitivity, bool linear)
        {
            //var sensitivityPercent = (sensitivity / 100.0);
            if (_isLinear) return (long)(value * _scaleFactor);

            //var sens = _scaleFactor / 100d;
            //double AxisRange = 1d * (Constants.AxisMaxValue - Constants.AxisMinValue);
            // Map value to -1 .. 1
            double val11 = (((value - Constants.AxisMinValue) / _axisRange) * 2) - 1;
            // calculate (Sensitivity * Value) + ( (1-Sensitivity) * Value^3 )
            double valout = (_sens * val11) + ((1 - _sens) * Math.Pow(val11, 3));
            // Map value back to AxisRange
            value = (long)Math.Round(((valout + 1) / 2d) * _axisRange + (1d * Constants.AxisMinValue));

            return value;
        }

    }
}
