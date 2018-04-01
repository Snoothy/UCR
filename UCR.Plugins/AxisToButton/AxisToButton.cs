using System;
using System.Collections.Generic;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.AxisToButton
{
    [Plugin("Axis to button")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button high")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button low")]
    public class AxisToButton : Plugin
    {
        [PluginGui("Invert", ColumnOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("Dead zone", ColumnOrder = 1)]
        public int DeadZone { get; set; }

        public AxisToButton()
        {
            DeadZone = 30;
        }

        public override void Update(params long[] values)
        {
            var value = values[0];
            if (Invert) value *= -1;
            value = Math.Sign(Functions.ApplyRangeDeadZone(value,DeadZone));
            switch (value)
            {
                case 0:
                    WriteOutput(0, 0);
                    WriteOutput(1, 0);
                    break;
                case 1:
                    WriteOutput(0, 1);
                    WriteOutput(1, 0);
                    break;
                case -1:
                    WriteOutput(0, 0);
                    WriteOutput(1, 1);
                    break;
            }
        }
    }
}
