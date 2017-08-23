using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Providers;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public enum KeyType
    {
        Button = 0,
        Axis = 1,
        Pov = 2
    }

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

        protected override InputType MapDeviceBindingInputType(DeviceBinding deviceBinding)
        {
            switch ((KeyType)deviceBinding.KeyType)
            {
                case KeyType.Button:
                    return InputType.BUTTON;
                case KeyType.Axis:
                    return InputType.AXIS;
                case KeyType.Pov:
                    return InputType.POV;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
