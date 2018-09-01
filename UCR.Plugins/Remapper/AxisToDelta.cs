using System;
using System.Diagnostics;
using System.Timers;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to Delta")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Delta, "Delta")]
    public class AxisToDelta : Plugin
    {
        [PluginGui("Invert", RowOrder = 0, ColumnOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("Dead zone", RowOrder = 0, ColumnOrder = 1)]
        public int DeadZone { get; set; }


        [PluginGui("Sensitivity", RowOrder = 0, ColumnOrder = 2)]
        public int Sensitivity { get; set; }

        [PluginGui("Min", RowOrder = 1, ColumnOrder = 0)]
        public int Min { get; set; }

        [PluginGui("Max", RowOrder = 1, ColumnOrder = 1)]
        public int Max { get; set; }

        private static Timer _absoluteModeTimer;
        private long _currentDelta;
        private float _scaleFactor;
        private readonly DeadzoneHelper _deadzoneHelper = new DeadzoneHelper();

        public AxisToDelta()
        {
            DeadZone = 0;
            Sensitivity = 1;
            Min = 1;
            Max = 20;
            _absoluteModeTimer = new Timer(10);
            _absoluteModeTimer.Elapsed += AbsoluteModeTimerElapsed;
        }

        public override void OnPropertyChanged()
        {
            base.OnPropertyChanged();
            PrecalculateValues();
        }

        private void PrecalculateValues()
        {
            _scaleFactor = (float)(Max - (Min - 1)) / 32767;
            _deadzoneHelper.Percentage = DeadZone;
        }

        public override void Update(params long[] values)
        {
            var value = values[0];
            if (value != 0) value = _deadzoneHelper.ApplyRangeDeadZone(value);
            if (Invert) value *= -1;

            if (value == 0)
            {
                SetAbsoluteTimerState(false);
                _currentDelta = 0;
            }
            else
            {
                var sign = Math.Sign(value);
                
                if (Sensitivity != 100) value = Functions.ApplyRangeSensitivity(value, Sensitivity, false);
                value = Math.Min(Math.Max(value, Constants.AxisMinValue), Constants.AxisMaxValue);
                _currentDelta = (long)(Min + (Math.Abs(value) * _scaleFactor)) * sign;
                //Debug.WriteLine($"New Delta: {_currentDelta}");
                SetAbsoluteTimerState(true);
            }
        }

        public override void OnActivate()
        {
            base.OnActivate();
            PrecalculateValues();
            if (_currentDelta != 0)
            {
                SetAbsoluteTimerState(true);
            }
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            SetAbsoluteTimerState(false);
        }

        public void SetAbsoluteTimerState(bool state)
        {
            if (state && !_absoluteModeTimer.Enabled)
            {
                _absoluteModeTimer.Start();
            }
            else if (!state && _absoluteModeTimer.Enabled)
            {
                _absoluteModeTimer.Stop();
            }
        }

        private void AbsoluteModeTimerElapsed(object sender, ElapsedEventArgs e)
        {
            WriteOutput(0, _currentDelta);
        }

    }
}
