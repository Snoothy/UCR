using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCR.Models.Devices
{
    public class DeviceGroup<T> where T : Device
    {
        // Guid used for persistance
        public String GUID { get; set; }

        // Runtime objects
        public List<T> Devices { get; set; }

        public DeviceGroup()
        {
            Devices = new List<T>();
        }

        public static DeviceGroup<T> FindDeviceGroup<T>(List<DeviceGroup<T>> deviceGroups, String GUID) where T : Device
        {
            if (GUID == null) return null;
            return deviceGroups?.FirstOrDefault(deviceGroup => deviceGroup.GUID == GUID);
        }
    }
}
