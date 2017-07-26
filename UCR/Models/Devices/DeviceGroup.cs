using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCR.Models.Devices
{
    public class DeviceGroup<T> where T : Device
    {
        // Id used for persistance
        public String Id { get; set; }

        // Runtime objects
        public List<T> Devices { get; set; }
    }
}
