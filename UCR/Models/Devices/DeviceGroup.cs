using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UCR.Models.Devices
{
    public class DeviceGroup
    {
        // Guid used for persistance
        public string Title { get; set; }
        public Guid Guid { get; set; }
        public List<Device> Devices { get; set; }

        public DeviceGroup(string title = null)
        {
            Title = title;
            Guid = Guid.NewGuid();
            Devices = new List<Device>();
        }

        public static DeviceGroup FindDeviceGroup(List<DeviceGroup> deviceGroups, Guid Guid)
        {
            return deviceGroups?.FirstOrDefault(deviceGroup => deviceGroup.Guid == Guid);
        }
    }
}
