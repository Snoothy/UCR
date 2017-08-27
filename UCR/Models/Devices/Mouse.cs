using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Providers;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public sealed class Mouse : Device
    {
        public Mouse() : base(DeviceType.Mouse)
        {
        }

        Mouse(Mouse mouse) : base(mouse)
        {
            // TODO copy vars
        }
    }
}
