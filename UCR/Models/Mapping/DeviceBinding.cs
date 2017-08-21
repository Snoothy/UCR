using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Devices;
using UCR.Models.Plugins;

namespace UCR.Models.Mapping
{
    public enum DeviceBindingType
    {
        Input,
        Output
    }

    public class DeviceBinding
    {
        // Persistence
        // Keyboard, mouse, joystick
        public DeviceType DeviceType { get; set; }
        // Index in its device list
        public int DeviceNumber { get; set; }
        // Subscription key
        public int KeyType { get; set; }
        public int KeyValue { get; set; }

        // Runtime
        public Guid Guid { get; }
        public Plugin Plugin { get; set; }
        public DeviceBindingType DeviceBindingType { get; }
        public bool IsBound { get; private set; }

        public delegate void ValueChanged(long value);
        public ValueChanged Callback { get; set; }

        public DeviceBinding(ValueChanged callback, Plugin plugin, DeviceBindingType deviceBindingType)
        {
            Callback = callback;
            Plugin = plugin;
            DeviceBindingType = deviceBindingType;
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
        }

        public void SetDeviceNumber(int number)
        {
            DeviceNumber = number;
            Plugin.BindingCallback(Plugin);
        }

        public void SetKeyTypeValue(int type, int value)
        {
            KeyType = type;
            KeyValue = value;
            IsBound = true;
            Plugin.BindingCallback(Plugin);
        }
    }
}
