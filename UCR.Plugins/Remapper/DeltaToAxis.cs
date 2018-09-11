using System;
using System.Timers;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Delta to Axis")]
    [PluginInput(DeviceBindingCategory.Delta, "Delta")]
    [PluginInput(DeviceBindingCategory.Momentary, "Reset")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class DeltaToAxis : Plugin
    {
        [PluginGui("Deadzone", ColumnOrder = 0, RowOrder = 0)]
        public int Deadzone { get; set; }

        [PluginGui("Relative Sensitivity", ColumnOrder = 1, RowOrder = 0)]
        public double RelativeSensitivity { get; set; }

        [PluginGui("Absolute Mode", ColumnOrder = 0, RowOrder = 1)]
        public bool AbsoluteMode { get; set; }

        [PluginGui("Absolute Sensitivity", ColumnOrder = 1, RowOrder = 1)]
        public double AbsoluteSensitivity { get; set; }

        [PluginGui("Absolute Timeout", ColumnOrder = 2, RowOrder = 1)]
        public int AbsoluteTimeout { get; set; }

        private long _currentValue;
        private static Timer _absoluteModeTimer;

        public DeltaToAxis()
        {
            AbsoluteMode = false;
            Deadzone = 0;
            RelativeSensitivity = 100;
            AbsoluteSensitivity = 10000;
            AbsoluteTimeout = 100;
            _absoluteModeTimer = new Timer();
            _absoluteModeTimer.Elapsed += AbsoluteModeTimerElapsed;
        }

        private void AbsoluteModeTimerElapsed(object sender, ElapsedEventArgs e)
        {
            WriteOutput(0, 0);
            SetAbsoluteTimerState(false);
        }

        public override void Update(params long[] values)
        {
            long value;
            if (values[1] == 1)
            {
                //ToDo: It is impossible to distinguish between release of Reset button, and axis input. Review.
                // Reset button pressed
                value = 0;
            }
            else
            {
                // Normal operation
                if (Math.Abs(values[0]) < Deadzone) return;
                if (AbsoluteMode)
                {
                    value = (long)(values[0] * AbsoluteSensitivity);
                    SetAbsoluteTimerState(true);
                }
                else
                {
                    value = _currentValue + (long)(values[0] * RelativeSensitivity);
                }
                value = Math.Min(Math.Max(value, Constants.AxisMinValue), Constants.AxisMaxValue);
            }
            _currentValue = value;
            WriteOutput(0, value);
        }

        public void SetAbsoluteTimerState(bool state)
        {
            if (state)
            {
                if (_absoluteModeTimer.Enabled)
                {
                    _absoluteModeTimer.Stop();
                }
                _absoluteModeTimer.Interval = AbsoluteTimeout;
                _absoluteModeTimer.Start();
            }
            else if (_absoluteModeTimer.Enabled)
            {
                _absoluteModeTimer.Stop();
            }
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            SetAbsoluteTimerState(false);
        }
    }
}
