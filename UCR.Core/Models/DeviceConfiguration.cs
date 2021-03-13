using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace HidWizards.UCR.Core.Models
{
    public class DeviceConfiguration
    {
        [XmlAttribute]
        public Guid Guid { get; set; }
        public Device Device { get; set; }
        [XmlAttribute]
        public string ConfigurationName { get; set; }
        public List<Device> ShadowDevices { get; set; }

        [XmlIgnore] 
        public int DeviceCount => 1 + ShadowDevices.Count;

        public DeviceConfiguration()
        {
            Guid = Guid.NewGuid();
        }

        public DeviceConfiguration(Device device) : this()
        {
            Device = device;
            ConfigurationName = null;
            ShadowDevices = new List<Device>();
        }

        public void ChangeConfigurationName(string name)
        {
            Device.Profile.Context.ContextChanged();
            if (string.IsNullOrEmpty(name))
            {
                ConfigurationName = null;
                return;
            }

            ConfigurationName = name;
        }

        public void ChangeShadowDevices(List<Device> shadowDevices)
        {
            Device.Profile.Context.ContextChanged();
            ShadowDevices = shadowDevices;
        }

        public List<Device> getAvailableShadowDevices(DeviceIoType deviceIoType)
        {
            var availableDevices = Device.Profile.Context.DevicesManager.GetAvailableDevicesListFromSameProvider(deviceIoType, Device);
            return availableDevices.Where(d => !d.Equals(Device)).ToList();
        }

        public string GetFullTitleForProfile(Profile profile)
        {
            var title = ConfigurationName ?? Device.Title;
            if (profile == null || Device.Profile.Guid == profile.Guid) return ConfigurationName ?? Device.Title;

            return $"{title} (Inherited from {Device.Profile.Title})";
        }
    }
}
