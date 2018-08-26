using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to axis (Delta)")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisToDelta : Plugin
    {
        [PluginGui("Invert", ColumnOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("Dead zone", ColumnOrder = 1)]
        public int DeadZone { get; set; }

        [PluginGui("Sensitivity", ColumnOrder = 2)]
        public int Sensitivity { get; set; }

        [PluginGui("Multiplier", ColumnOrder = 3)]
        public int Multiplier { get; set; }

        [PluginGui("Curve", ColumnOrder = 4)]
        public double Curve { get; set; }

        private long AxisRest;

        public AxisToDelta()
        {
            DeadZone = 0;
            Sensitivity = 100;
            Multiplier = 20;
            Curve = 1.2;
            AxisRest = 0;
        }

        public override void Update(params long[] values)
        {
            var value = values[0];
            var delta = value - AxisRest;

            // Alter the response
            delta = delta * Multiplier;

            // Invert
            if (Invert) delta *= -1;

            // Apply a deadzone
            if (DeadZone != 0) delta = Functions.ApplyRangeDeadZone(delta, DeadZone);

            // Apply a linear sensitivity to the input
            // (only "linear response" has some use dealing with deltas...)
            if (Sensitivity != 100) delta = Functions.ApplyRangeSensitivity(delta, Sensitivity, true);

            // Apply a curved response to the output
            if (Curve != 0) delta = Functions.ApplyCurvedResponse(delta, Curve);

            // Kepp output in range
            delta = Math.Min(Math.Max(delta, Constants.AxisMinValue), Constants.AxisMaxValue);
            WriteOutput(0, delta);

            AxisRest = (long)value;
        }
    }
}