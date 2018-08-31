using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis Initializer")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisInitializer : Plugin
    {
        [PluginGui("Initialize value (0 to 100)", ColumnOrder = 0, RowOrder = 0)]
        public int InitializeValue { get; set; }

        public AxisInitializer()
        {
        }

        public override void OnActivate()
        {
            base.OnActivate();
            var value = (long)Math.Min(Math.Max((InitializeValue * 655.35) - 32768, Constants.AxisMinValue), Constants.AxisMaxValue);
            WriteOutput(0, value);
        }

        public override void Update(params long[] values)
        {
            var value = values[0];
            value = Math.Min(Math.Max(value, Constants.AxisMinValue), Constants.AxisMaxValue);
            WriteOutput(0, value);
        }
    }
}
