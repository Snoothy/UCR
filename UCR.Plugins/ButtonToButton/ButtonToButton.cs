using System.Collections.Generic;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.ButtonToButton
{
    [Plugin("Button to button")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button")]
    public class ButtonToButton : Plugin
    {
        public override void Update(List<long> values)
        {
            WriteOutput(0, values[0]);
        }
    }
}
