using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Button to Button", Group = "Button", Description = "Map from one button to another")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button")]
    public class ButtonToButton : Plugin
    {

        [PluginGui("Invert")]
        public bool Invert { get; set; }

        public override void Update(params short[] values)
        {
            if (Invert)
            {
                WriteOutput(0, (short) (values[0] == 0 ? 1 : 0));
            }
            else
            {
                WriteOutput(0, values[0]);
            }
        }
    }
}
