using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public enum DeviceType
    {
        Keyboard,
        Mouse,
        Joystick
    }

    public abstract class Device
    {
        public String Title { get; set; }
        public String Guid { get; set; }
        public DeviceType DeviceType { get; set; }

        public abstract bool Subscribe(Binding binding);
        public abstract void ClearSubscribers();
    }

}
