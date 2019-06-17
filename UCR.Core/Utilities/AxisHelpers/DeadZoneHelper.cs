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
            {
                if (value < 0)
                {
                    _percentage = 0;
                }
                else if (value > 100)
                {
                    _percentage = 100;
                }
                else
                {
                    _percentage = value;
                }
                
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
                _deadzoneCutoff = Constants.AxisMaxValue * (_percentage * 0.01);
                _scaleFactor = Constants.AxisMaxValue / (Constants.AxisMaxValue - _deadzoneCutoff);
            }
        }

        public short ApplyRangeDeadZone(short value)
        {
            var wideVal = Functions.WideAbs(value);
            if (wideVal < Math.Round(_deadzoneCutoff))
            {
                return 0;
            }

            var sign = Math.Sign(value);
            var adjustedValue = (wideVal - _deadzoneCutoff) * _scaleFactor;
            var newValue = (int) Math.Round(adjustedValue * sign);
            if (newValue < -32768) newValue = -32768;   // ToDo: Negative values can go up to -32777 (9 over), can this be improved?
            //Debug.WriteLine($"Pre-DZ: {value}, Post-DZ: {newValue}, Cutoff: {_deadzoneCutoff}");
            return (short) newValue;
        }
    }
}
