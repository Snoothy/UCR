using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to axis (delta)")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisToDelta : Plugin
    {
        [PluginGui("Invert", ColumnOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("Linear", ColumnOrder = 3)]
        public bool Linear { get; set; }

        [PluginGui("Dead zone", ColumnOrder = 1)]
        public int DeadZone { get; set; }

        [PluginGui("Sensitivity", ColumnOrder = 2)]
        public int Sensitivity { get; set; }

        [PluginGui("Multiplier", ColumnOrder = 3)]
        public int Multiplier { get; set; }

        private long AxisRest;

        public AxisToDelta()
        {
            DeadZone = 0;
            Sensitivity = 100;
            Multiplier = 20;
            AxisRest = 0;
        }

        public override void Update(params long[] values)
        {
            var value = values[0];
            var delta = value - AxisRest;

            if (Invert) delta *= -1;
            if (DeadZone != 0) delta = Functions.ApplyRangeDeadZone(delta, DeadZone);
            if (Sensitivity != 100) delta = Functions.ApplyRangeSensitivity(delta, Sensitivity, Linear);

            delta = delta * Multiplier;

            delta = Math.Min(Math.Max(delta, Constants.AxisMinValue), Constants.AxisMaxValue);
            WriteOutput(0, delta);

            AxisRest = (long)value;
        }
    }
}