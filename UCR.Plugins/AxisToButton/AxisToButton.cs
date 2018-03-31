using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.AxisToButton
{
    [Plugin("Axis to button")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button")]
    public class AxisToButton : Plugin
    {
        public bool Invert { get; set; }

        private int _deadZoneValue;
        private string _deadZone;
        public string DeadZone
        {
            get { return _deadZone; }
            set
            {
                SetIntValue(ref _deadZoneValue, value);
                ContextChanged();
                _deadZone = value;
            }
        }

        private long _direction = 0;
        
        public AxisToButton()
        {
            DeadZone = "30";
        }

        public override void Update(List<long> values)
        {
            var value = values[0];
            if (Invert) value *= -1;
            if (value < 0) value = 0;
            value = Math.Sign(ApplyDeadZone(value));
            WriteOutput(0, value);
        }

        private long ApplyDeadZone(long value)
        {
            var gap = (_deadZoneValue / 100.0) * Constants.AxisMaxValue;
            var remainder = Constants.AxisMaxValue - gap;
            var gapPercent = Math.Max(0, Math.Abs(value) - gap) / remainder;
            return (long)(gapPercent * Constants.AxisMaxValue * Math.Sign(value));
        }

        private static void SetIntValue(ref int field, string value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                field = result;
            }
        }
    }
}
