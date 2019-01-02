using System;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginIoAttribute : PluginGroupAttribute
    {
        private DeviceIoType _deviceIoType;
        private DeviceBindingCategory _deviceBindingCategory;
        private string _name;

        public PluginIoAttribute(DeviceIoType deviceIoType, DeviceBindingCategory deviceBindingCategory, string name, string groupName) : base(groupName)
        {
            _deviceIoType = deviceIoType;
            _deviceBindingCategory = deviceBindingCategory;
            _name = name;
        }

        public virtual DeviceIoType DeviceIoType => _deviceIoType;

        public virtual DeviceBindingCategory DeviceBindingCategory => _deviceBindingCategory;

        public virtual string Name => _name;
    }
}
