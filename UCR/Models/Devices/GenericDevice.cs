using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Providers;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public sealed class GenericDevice : Device
    {
        public GenericDevice() : base(DeviceType.Mouse)
        {
        }
    }
}
