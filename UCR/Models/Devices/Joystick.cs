using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public enum InputType
    {
        DirectInput,
        XInput
    }

    public class Joystick : Device
    {
        public InputType InputType { get; }

        public Joystick(InputType inputType)
        {
            DeviceType = DeviceType.Joystick;
            InputType = inputType;
        }

        public override bool Subscribe(Binding binding)
        {
            throw new NotImplementedException();
        }

        public override void ClearSubscribers()
        {
            throw new NotImplementedException();
        }
    }
}
