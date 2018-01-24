using System;
using System.Collections.Generic;
using System.Linq;

namespace UCR.Core.Models.Device
{
    public class DeviceGroup
    {
        // Guid used for persistance
        public string Title { get; set; }
        public Guid Guid { get; set; }
        public List<Models.Device.Device> Devices { get; set; }

        private DeviceGroup()
        {
            Guid = Guid.NewGuid();
            Devices = new List<Models.Device.Device>();
        }

        public DeviceGroup(string title = null) : this()
        {
            Title = title;
        }

        public bool RemoveDevice(Guid guid)
        {
            return Devices.RemoveAll(d => d.Guid == guid) > 0;
        }

        public static DeviceGroup FindDeviceGroup(List<DeviceGroup> deviceGroups, Guid Guid)
        {
            return deviceGroups?.FirstOrDefault(deviceGroup => deviceGroup.Guid == Guid);
        }
    }
}
