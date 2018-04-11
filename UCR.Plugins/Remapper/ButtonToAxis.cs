using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Button to axis")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class ButtonToAxis : Plugin
    {
        [PluginGui("Invert", ColumnOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("Range target", ColumnOrder = 1)]
        public int Range { get; set; }

        public ButtonToAxis()
        {
            Range = 100;
        }

        public override void Update(params long[] values)
        {
            if (Invert) values[0] = values[0] * - 1;
            WriteOutput(0, values[0] * (long)(Constants.AxisMaxValue * ( Range / 100.0 )));
        }
    }
}
