using System.Collections.Generic;
using System.ComponentModel.Composition;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.ButtonToButton
{
    [Export(typeof(Plugin))]
    public class ButtonToButton : Plugin
    {
        public override string PluginName => "Button to Button";
        public override DeviceBindingCategory OutputCategory => DeviceBindingCategory.Momentary;
        protected override List<PluginInput> InputCategories => new List<PluginInput>()
        {
            new PluginInput()
            {
                Name = "Button",
                Category = DeviceBindingCategory.Momentary
            }
        };

        public override long Update(List<long> values)
        {
            return values[0];
        }
    }
}
