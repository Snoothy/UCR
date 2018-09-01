using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Button to Axis")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class ButtonToAxis : Plugin
    {
        [PluginGui("Invert", ColumnOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("Absolute", ColumnOrder = 1)]
        public bool Absolute { get; set; }

        [PluginGui("Range target", ColumnOrder = 2)]
        public int Range { get; set; }


        public ButtonToAxis()
        {
            Range = 100;
        }

        public override void Update(params long[] values)
        {
            var value = values[0];
            var inverse = value == 0 ^ Invert;

            if (Absolute)
            {
                WriteOutput(0, (long)((Constants.AxisMinValue + value * Constants.AxisMaxValue * 2 * (Range / 100.0))) * (Invert ? -1 : 1));
            }
            else
            {
                WriteOutput(0, value * (long)((inverse ? Constants.AxisMinValue : Constants.AxisMaxValue) * (Range / 100.0)));
            }
        }

    }
}
