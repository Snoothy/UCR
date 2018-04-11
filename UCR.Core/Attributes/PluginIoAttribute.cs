using System;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginIoAttribute : Attribute
    {
        private DeviceIoType deviceIoType;
        private DeviceBindingCategory deviceBindingCategory;
        private string name;

        public PluginIoAttribute(DeviceIoType deviceIoType, DeviceBindingCategory deviceBindingCategory, string name)
        {
            this.deviceIoType = deviceIoType;
            this.deviceBindingCategory = deviceBindingCategory;
            this.name = name;
        }

        public virtual DeviceIoType DeviceIoType => deviceIoType;

        public virtual DeviceBindingCategory DeviceBindingCategory => deviceBindingCategory;

        public virtual string Name => name;
    }
}
