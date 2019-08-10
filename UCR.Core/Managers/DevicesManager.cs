using System.Collections.Generic;
using System.Linq;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.Core.Managers
{
    public class DevicesManager
    {
        private readonly Context _context;

        public DevicesManager(Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a list of available devices from the backend
        /// </summary>
        /// <param name="type"></param>
        public List<DeviceGroup> GetAvailableDeviceList(DeviceIoType type)
        {
            var deviceGroupList = new List<DeviceGroup>();
            var providerList = type == DeviceIoType.Input
                ? _context.IOController.GetInputList()
                : _context.IOController.GetOutputList();

            foreach (var providerReport in providerList)
            {
                var deviceGroup = new DeviceGroup(providerReport.Key);
                foreach (var ioWrapperDevice in providerReport.Value.Devices)
                {
                    deviceGroup.Devices.Add(new Device(ioWrapperDevice, providerReport.Value, type));
                }
                deviceGroupList.Add(deviceGroup);
            }
            return deviceGroupList;
        }

        public List<Device> GetAvailableDevicesListFromSameProvider(DeviceIoType type, Device device)
        {
            var availableDeviceList = GetAvailableDeviceList(type);
            return availableDeviceList.SelectMany(deviceGroup => deviceGroup.Devices).Where(d => d.ProviderName.Equals(device.ProviderName)).ToList();
        }
    }
}