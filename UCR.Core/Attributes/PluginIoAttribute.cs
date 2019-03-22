using System;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginIoAttribute : PluginGroupAttribute
    {
        public virtual DeviceIoType DeviceIoType { get; }
        public virtual DeviceBindingCategory DeviceBindingCategory { get; }
        public virtual string Name { get; }

        public PluginIoAttribute(DeviceIoType deviceIoType, DeviceBindingCategory deviceBindingCategory, string name)
        {
            DeviceIoType = deviceIoType;
            DeviceBindingCategory = deviceBindingCategory;
            Name = name;
        }
    }
}
