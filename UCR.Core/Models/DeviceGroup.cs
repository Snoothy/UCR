using System;
using System.Collections.Generic;
using System.Linq;

namespace HidWizards.UCR.Core.Models
{
    public class DeviceGroup
    {
        // Guid used for persistence
        public string Title { get; set; }
        public Guid Guid { get; set; }
        public List<Device> Devices { get; set; }

        private DeviceGroup()
        {
            Guid = Guid.NewGuid();
            Devices = new List<Device>();
        }

        public DeviceGroup(string title = null) : this()
        {
            Title = title;
        }
    }
}
