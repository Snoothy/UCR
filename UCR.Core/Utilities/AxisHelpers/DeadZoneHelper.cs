using System;
using System.Diagnostics;

namespace HidWizards.UCR.Core.Utilities.AxisHelpers
{
    public class DeadZoneHelper
    {
        //private double gapPercent;
        private double _scaleFactor;
        private double _deadzoneCutoff;
        
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

        public DeadZoneHelper()
        {
            PrecalculateValues();
        }

        private void PrecalculateValues()
        {
            if (_percentage == 0)
            {
                _deadzoneCutoff = 0;
                _scaleFactor = 1.0;
            }
            else
            {
                _deadzoneCutoff = (Constants.AxisMaxValue - (Constants.AxisMaxValue * (_percentage / 100.0)));
                _scaleFactor = Math.Round(Constants.AxisMaxValue / _deadzoneCutoff);
            }
        }

        public long ApplyRangeDeadZone(long value)
        {
            var absValue = Math.Abs(value);
            if (absValue < Math.Round(_deadzoneCutoff))
            {
                return 0;
            }

            var sign = Math.Sign(value);
            var adjustedValue = (absValue - _deadzoneCutoff) * _scaleFactor;
            var newValue = (long) Math.Round(adjustedValue * sign);
            if (newValue == -32769) newValue = -32768;
            //Debug.WriteLine($"Pre-DZ: {value}, Post-DZ: {newValue}, Cutoff: {_deadzoneCutoff}");
            return newValue;
        }
    }
}
