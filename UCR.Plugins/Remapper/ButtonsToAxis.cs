using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Buttons to Axis", Group = "Axis", Description = "Map two buttons to one axis")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button (Low)")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button (High)")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class ButtonsToAxis : Plugin
    {
        [PluginGui("Invert")]
        public bool Invert { get; set; }

        public override void Update(params short[] values)
        {
            short value;

            if (values[0] == 1 && values[1] == 1)
            {
                value = 0;
            }
            else if (values[0] == 1)
            {
                value = Constants.AxisMaxValue;
            }
            else if (values[1] == 1)
            {
                value = Constants.AxisMinValue;
            }
            else
            {
                value = 0;
            }

            if (Invert) value = Functions.Invert(value);
            WriteOutput(0, value);
        }

    }
}
