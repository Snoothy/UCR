using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var x = values[0];
            var y = values[1];

            var inputRadius = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            var inputAngle = Math.Atan2(x, y);
            if (inputRadius < _deadzoneRadius)
            {
                return new short[] { 0, 0 };
            }
            var adjustedRadius = inputRadius - _deadzoneRadius;
            var outputRadius = adjustedRadius * _scaleFactor;

            var outX = (int)(outputRadius * Math.Sin(inputAngle));
            var outY = (int)(outputRadius * Math.Cos(inputAngle));
            if (outX == -32769) outX = -32768;
            if (outY == -32769) outY = -32768;

            var output = new[] { (short)outX, (short)outY };
            return output;

        }
    }
}
