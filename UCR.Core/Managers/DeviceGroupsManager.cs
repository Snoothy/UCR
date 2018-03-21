using System;
using System.Collections.Generic;
using System.Linq;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.Core.Managers
{
    public class DeviceGroupsManager
    {
        private readonly Context Context;
        private readonly List<DeviceGroup> InputGroups;
        private readonly List<DeviceGroup> OutputGroups;

        public DeviceGroupsManager(Context context, List<DeviceGroup> inputGroups, List<DeviceGroup> outputGroups)
        {
            Context = context;
            InputGroups = inputGroups;
            OutputGroups = outputGroups;
        }

        public DeviceGroup GetDeviceGroup(DeviceIoType deviceIoType, Guid deviceGroupGuid)
        {
            return GetDeviceGroupList(deviceIoType).FirstOrDefault(d => d.Guid == deviceGroupGuid) ?? new DeviceGroup();
        }

        public Guid AddDeviceGroup(string Title, DeviceIoType deviceIoType)
        {
            var deviceGroup = new DeviceGroup(Title);
            GetDeviceGroupList(deviceIoType).Add(deviceGroup);
            Context.ContextChanged();
            return deviceGroup.Guid;
        }

        public bool RemoveDeviceGroup(Guid deviceGroupGuid, DeviceIoType deviceIoType)
        {
            var deviceGroups = GetDeviceGroupList(deviceIoType);
            if (!deviceGroups.Remove(DeviceGroup.FindDeviceGroup(deviceGroups, deviceGroupGuid))) return false;
            Context.ContextChanged();
            return true;
        }

        public bool RenameDeviceGroup(Guid deviceGroupGuid, DeviceIoType deviceIoType, string title)
        {
            var deviceGroups = GetDeviceGroupList(deviceIoType);
            DeviceGroup.FindDeviceGroup(deviceGroups, deviceGroupGuid).Title = title;
            Context.ContextChanged();
            return true;
        }

        public void AddDeviceToDeviceGroup(Device device, DeviceIoType deviceIoType, Guid deviceGroupGuid)
        {
            GetDeviceGroupList(deviceIoType).First(d => d.Guid == deviceGroupGuid).Devices.Add(device);
            Context.ContextChanged();
        }

        public void RemoveDeviceFromDeviceGroup(Device device, DeviceIoType deviceIoType, Guid deviceGroupGuid)
        {
            GetDeviceGroup(deviceIoType, deviceGroupGuid).RemoveDevice(device.Guid);
            Context.ContextChanged();
        }

        public List<DeviceGroup> GetDeviceGroupList(DeviceIoType deviceIoType)
        {
            switch (deviceIoType)
            {
                case DeviceIoType.Input:
                    return InputGroups;
                case DeviceIoType.Output:
                    return OutputGroups;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deviceIoType), deviceIoType, null);
            }
        }
    }
}
