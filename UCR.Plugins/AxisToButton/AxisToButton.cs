using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Serialization;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.AxisToButton
{
    [Export(typeof(Plugin))]
    public class AxisToButton : Plugin
    {
        public override string PluginName => "Axis to button";
        public override DeviceBindingCategory OutputCategory => DeviceBindingCategory.Momentary;
        protected override List<PluginInput> InputCategories => new List<PluginInput>()
        {
            new PluginInput()
            {
                Name = "Axis",
                Category = DeviceBindingCategory.Range
            }
        };

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


        // TODO implement high/low select
        public override long Update(List<long> values)
        {
            var value = values[0];
            if (Invert) value *= -1;
            value = Math.Sign(ApplyDeadZone(value));
            //switch (value)
            //{
            //    case 0:
            //        WriteOutput(OutputLow, 0);
            //        WriteOutput(OutputHigh, 0);
            //        break;
            //    case -1:
            //        WriteOutput(OutputLow, 1);
            //        WriteOutput(OutputHigh, 0);
            //        break;
            //    case 1:
            //        WriteOutput(OutputLow, 0);
            //        WriteOutput(OutputHigh, 1);
            //        break;
            //}
            _direction = value;
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
