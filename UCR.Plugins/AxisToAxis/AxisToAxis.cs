using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.AxisToAxis
{
    [Export(typeof(Plugin))]
    public class AxisToAxis : Plugin
    {
        public override string PluginName => "Axis to axis";
        public override DeviceBindingCategory OutputCategory => DeviceBindingCategory.Range;
        protected override List<PluginInput> InputCategories => new List<PluginInput>()
        {
            new PluginInput()
            {
                Name = "Axis",
                Category = DeviceBindingCategory.Range
            }
        };

        public bool Invert { get; set; }
        public bool Linear { get; set; }

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

        private int _sensitivityValue;
        private string _sensitivity;
        public string Sensitivity
        {
            get { return _sensitivity; }
            set
            {
                SetIntValue(ref _sensitivityValue, value);
                ContextChanged();
                _sensitivity = value;
            }
        }

        public AxisToAxis()
        {
            DeadZone = "0";
            Sensitivity = "100";
        }

        public override long Update(List<long> values)
        {
            var value = values[0];
            if (Invert) value *= -1;
            if (_deadZoneValue != 0) value = ApplyDeadZone(value);
            if (_sensitivityValue != 100) value = ApplySensitivity(value);
            return Math.Min(Math.Max(value, Constants.AxisMinValue), Constants.AxisMaxValue);
        }

        private long ApplySensitivity(long value)
        {
            var sensitivityPercent = (_sensitivityValue / 100.0);
            if (Linear) return (long) (value * sensitivityPercent);
            // TODO https://github.com/evilC/UCR/blob/master/Libraries/StickOps/StickOps.ahk#L60
            return value;
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
