using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Button to Filter", Group = "Button", Description = "Change filter state using a button")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    public class ButtonToFilter : Plugin
    {
        [PluginGui("Filter name")]
        public string FilterName { get; set; }

        public override void Update(params short[] values)
        {
            WriteFilterState(FilterName, values[0] == 1);
        }
    }
}
