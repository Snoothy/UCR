using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public class Keyboard : Device
    {
        public Keyboard()
        {
            DeviceType = DeviceType.Keyboard;
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
