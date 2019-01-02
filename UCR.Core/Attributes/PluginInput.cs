using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Attributes
{
    public class PluginInput : PluginIoAttribute
    {
        public PluginInput(DeviceBindingCategory deviceBindingCategory, string name) : base(DeviceIoType.Input, deviceBindingCategory, name, null)
        {
        }
    }
}
