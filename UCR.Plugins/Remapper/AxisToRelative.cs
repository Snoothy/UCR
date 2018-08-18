using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to relative")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisToRelative : Plugin
    {
        [PluginGui("Invert", ColumnOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("Dead zone", ColumnOrder = 1)]
        public int DeadZone { get; set; }

        [PluginGui("Sensitivity", ColumnOrder = 2)]
        public int Sensitivity { get; set; }

        /// <summary>
        /// To constantly add current axis values to the output - WORK IN PROGRESS!!!
        /// </summary>
        [PluginGui("Continue", ColumnOrder = 3)]
        public bool Continue { get; set; }

        /// <summary>
        /// Create the previous values field, which will be updated on each iteration.
        /// </summary>
        private long previousValue = 0;

        public AxisToRelative()
        {
            DeadZone = 0;
            Sensitivity = 100;
            Continue = false;
        }

        public override void Update(params long[] values)
        {
            var value = values[0];

            if (Invert) value *= -1;
            if (DeadZone != 0) value = Functions.ApplyRangeDeadZone(value, DeadZone);
            if (Sensitivity != 100) value = Functions.ApplyRangeSensitivity(value, Sensitivity, false);

            if (Continue) value = Functions.ApplyContinueRelativeIncrement(value, previousValue, Sensitivity);
            else value = Functions.ApplyRelativeIncrement(value, previousValue, Sensitivity);

            // Respect the axis min and max ranges.
            value = Math.Min(Math.Max(value, Constants.AxisMinValue), Constants.AxisMaxValue);

            WriteOutput(0, value);

            // Store the last value and use it as previous, on next iteration.
            previousValue = value;
        }
    }
}