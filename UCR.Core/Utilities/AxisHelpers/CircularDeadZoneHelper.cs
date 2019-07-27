using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HidWizards.UCR.Core.Utilities.AxisHelpers
{
    public class CircularDeadZoneHelper
    {
        private double _scaleFactor;
        private double _deadzoneRadius;

        public int Percentage
        {
            get => _percentage;
            set
            //TODO CHECK FOR NEG
            {
                _percentage = value;
                PrecalculateValues();
            }
        }
        private int _percentage;

        public CircularDeadZoneHelper()
        {
            PrecalculateValues();
        }

        private void PrecalculateValues()
        {
            if (_percentage == 0)
            {
                _deadzoneRadius = 0;
                _scaleFactor = 1;
            }
            else
            {
                const double max = Constants.AxisMaxValue;
                _deadzoneRadius = (_percentage / 100d) * max;
                _scaleFactor = max / (max - _deadzoneRadius);
            }
        }

        public short[] ApplyRangeDeadZone(short[] values)
        {
            var vector = new Vector(values[0], values[1]);
            var originalLength = vector.Length;
            if (originalLength <= _deadzoneRadius) return new short[]{ 0, 0 };
            var newLength = (originalLength - _deadzoneRadius) / (Constants.AxisMaxValue - _deadzoneRadius);
            var result = vector * (newLength / originalLength) * Constants.AxisMaxValue;

            return new[] { Functions.ClampAxisRange((int)result.X), Functions.ClampAxisRange((int)result.Y) };
        }
    }
}
