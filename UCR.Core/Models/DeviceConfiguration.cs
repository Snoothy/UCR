using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidWizards.UCR.Core.Models
{
    public class DeviceConfiguration
    {
        public Device Device { get; set; }
        public string ConfigurationName { get; set; }
        public List<Device> ShadowDevices { get; set; }

        public DeviceConfiguration()
        {
        }

        public DeviceConfiguration(Device device)
        {
            Device = device;
            ConfigurationName = null;
            ShadowDevices = new List<Device>();
        }
    }
}
