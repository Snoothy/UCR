﻿using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to Button")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button high")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button low")]
    public class AxisToButton : Plugin
    {
        [PluginGui("Invert", ColumnOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("Dead zone", ColumnOrder = 1)]
        public int DeadZone { get; set; }

        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();

        public AxisToButton()
        {
            DeadZone = 30;
        }

        public override void InitializeCacheValues()
        {
            Initialize();
        }

        public override void Update(params long[] values)
        {
            var value = values[0];
            if (Invert) value = Functions.Invert(value);
            value = Math.Sign(_deadZoneHelper.ApplyRangeDeadZone(value));
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

        private void Initialize()
        {
            _deadZoneHelper.Percentage = DeadZone;
        }
    }
}
