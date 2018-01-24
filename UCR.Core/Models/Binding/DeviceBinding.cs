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
        [XmlIgnore]
        public ValueChanged OutputSink { get; set; }

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
        }

        public DeviceBinding(DeviceBinding deviceBinding)
        {
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
            Plugin.ParentProfile.context.ContextChanged();
        }

        public void SetKeyTypeValue(int type, int value, int subValue)
        {
            KeyType = type;
            KeyValue = value;
            KeySubValue = subValue;
            IsBound = true;
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

        public void WriteOutput(long value)
        {
            OutputSink?.Invoke(value);
        }
    }
}
