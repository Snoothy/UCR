using System;

namespace HidWizards.UCR.Core.Utilities.AxisHelpers
{
    public class AntiDeadZoneHelper
    {
        private double _scaleFactor;
        private double _antiDeadzoneStart;
        
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

        public AntiDeadZoneHelper()
        {
            PrecalculateValues();
        }

        private void PrecalculateValues()
        {
            if (_percentage == 0)
            {
                _antiDeadzoneStart = 0;
                _scaleFactor = 1.0;
            }
            else
            {
                _antiDeadzoneStart = Constants.AxisMaxValue * (_percentage * 0.01);
                _scaleFactor = (Constants.AxisMaxValue - _antiDeadzoneStart) / Constants.AxisMaxValue;
            }
        }

        public short ApplyRangeAntiDeadZone(short value)
        {
            if (value == 0) return 0;
            
            var wideVal = Functions.WideAbs(value);

            var sign = Math.Sign(value);
            var adjustedValue = _antiDeadzoneStart + (wideVal * _scaleFactor);
            var newValue = (int) Math.Round(adjustedValue * sign);
            
            // TODO: Negative values can go up to -32777 (9 over), can this be improved?
            if (newValue < Constants.AxisMinValue) newValue = Constants.AxisMinValue;   
            return (short) newValue;
        }
    }
}
