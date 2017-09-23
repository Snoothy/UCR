using System;
using System.Xml.Serialization;
using Providers;
using UCR.Core.Models.Device;

namespace UCR.Core.Models.Binding
{
    public enum DeviceBindingCategory
    {
        Event,
        Momentary,
        Range,
        Delta
    }

    public class DeviceBinding
    {
        // Persistence
        public bool IsBound { get; set; }
        // Keyboard, mouse, joystick
        public DeviceType DeviceType { get; set; }
        // Index in its device list
        public int DeviceNumber { get; set; }
        // Subscription key
        public int KeyType { get; set; }
        public int KeyValue { get; set; }
        public int KeySubValue { get; set; }

        // Runtime
        [XmlIgnore]
        public Guid Guid { get; }
        [XmlIgnore]
        public Models.Plugin.Plugin Plugin { get; set; }
        [XmlIgnore]
        public DeviceIoType DeviceIoType { get; set; }
        [XmlIgnore]
        public DeviceBindingCategory DeviceBindingCategory { get; set; }


        public delegate void ValueChanged(long value);
        [XmlIgnore]
        public ValueChanged Callback { get; set; }

        private DeviceBinding()
        {
            Guid = Guid.NewGuid();
        }

        public DeviceBinding(ValueChanged callback, Plugin.Plugin plugin, DeviceIoType deviceIoType)
        {
            Callback = callback;
            Plugin = plugin;
            DeviceIoType = deviceIoType;
            Guid = Guid.NewGuid();
            IsBound = false;
            DeviceType = DeviceType.Joystick;
        }

        public DeviceBinding(DeviceBinding deviceBinding)
        {
            DeviceType = deviceBinding.DeviceType;
            DeviceNumber = deviceBinding.DeviceNumber;
            KeyType = deviceBinding.KeyType;
            KeyValue = deviceBinding.KeyValue;
            Plugin = deviceBinding.Plugin;
            Callback = deviceBinding.Callback;
            Guid = deviceBinding.Guid;
            IsBound = deviceBinding.IsBound;
        }

        public void SetDeviceNumber(int number)
        {
            DeviceNumber = number;
            if (DeviceIoType == DeviceIoType.Input) Plugin.BindingCallback(Plugin);
            Plugin.ParentProfile.context.ContextChanged();
        }

        public void SetKeyTypeValue(int type, int value, int subValue)
        {
            KeyType = type;
            KeyValue = value;
            KeySubValue = subValue;
            IsBound = true;
            if (DeviceIoType == Device.DeviceIoType.Input) Plugin.BindingCallback(Plugin);
            Plugin.ParentProfile.context.ContextChanged();
        }

        public void SetDeviceType(DeviceType deviceType)
        {
            // Unsubscribe old input
            IsBound = false;
            if (DeviceIoType == DeviceIoType.Input) Plugin.BindingCallback(Plugin);

            // Set new input
            DeviceType = deviceType;
            KeyType = 0;
            KeyValue = 0;
            KeySubValue = 0;
            Plugin.ParentProfile.context.ContextChanged();
        }
        
        public string BoundName()
        {
            return Plugin.GetDevice(this)?.GetBindingName(this) ?? "Device unavailable";
        }

        public static DeviceBindingCategory MapCategory(BindingCategory bindingInfoCategory)
        {
            switch (bindingInfoCategory)
            {
                case BindingCategory.Event:
                    return DeviceBindingCategory.Event;
                case BindingCategory.Momentary:
                    return DeviceBindingCategory.Momentary;
                case BindingCategory.Signed:
                case BindingCategory.Unsigned:
                    return DeviceBindingCategory.Range;
                case BindingCategory.Delta:
                    return DeviceBindingCategory.Delta;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bindingInfoCategory), bindingInfoCategory, null);
            }
        }
    }
}
