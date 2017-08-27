using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Providers;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public sealed class Keyboard : Device
    {
        public Keyboard() : base(DeviceType.Keyboard)
        {
        }

        public Keyboard(Keyboard keyboard) : base(keyboard)
        {
            // TODO Copy vars
        }
    }
}
