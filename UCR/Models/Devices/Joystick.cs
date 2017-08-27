using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Providers;
using UCR.Models.Mapping;
using BindingInfo = Providers.BindingInfo;

namespace UCR.Models.Devices
{

    public sealed class Joystick : Device
    {        
        public Joystick() : base(DeviceType.Joystick)
        {
            ClearSubscribers();
        }

        public Joystick(Joystick joystick) : base(joystick)
        {
            ClearSubscribers();
        }

    }
}
