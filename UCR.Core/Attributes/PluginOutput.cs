using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Attributes
{
    public class PluginOutput : PluginIoAttribute
    {
        public PluginOutput(DeviceBindingCategory deviceBindingCategory, string name) : base(DeviceIoType.Output, deviceBindingCategory, name)
        {
        }
    }
}
