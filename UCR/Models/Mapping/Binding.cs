using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Devices;

namespace UCR.Models.Mapping
{
    public enum BindingType
    {
        Input,
        Output
    }

    public class Binding
    {
        // Persistence
        // Keyboard, mouse, joystick
        public DeviceType? DeviceType { get; set; }
        // Index in its device list
        public int DeviceNumber { get; set; }
        // Subscription key
        public int KeyType { get; set; }
        public int KeyValue { get; set; }

        // Runtime
        public String PluginName { get; set; }
        public delegate void ValueChanged(long value);
        public ValueChanged Callback { get; set; }

        public Binding(ValueChanged callback)
        {
            Callback = callback;
        }
    }
}
