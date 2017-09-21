using System;
using System.Collections.Generic;
using System.Linq;
using UCR.Core.Models.Device;

namespace UCR.Core.Controllers
{
    public class DeviceGroupsController
    {
        private readonly Context Context;
        private readonly List<DeviceGroup> JoystickGroups;
        private readonly List<DeviceGroup> KeyboardGroups;
        private readonly List<DeviceGroup> MiceGroups;
        private readonly List<DeviceGroup> GenericDeviceGroups;

        public DeviceGroupsController(Context context, List<DeviceGroup> joystickGroups, List<DeviceGroup> keyboardGroups, List<DeviceGroup> miceGroups, List<DeviceGroup> genericDeviceGroups)
        {
            Context = context;
            JoystickGroups = joystickGroups;
            KeyboardGroups = keyboardGroups;
            MiceGroups = miceGroups;
            GenericDeviceGroups = genericDeviceGroups;
        }

        public DeviceGroup GetDeviceGroup(DeviceType deviceType, Guid deviceGroupGuid)
        {
            return GetDeviceGroupList(deviceType).FirstOrDefault(d => d.Guid == deviceGroupGuid);
        }

        public Guid AddDeviceGroup(string Title, DeviceType deviceType)
        {
            var deviceGroup = new DeviceGroup(Title);
            GetDeviceGroupList(deviceType).Add(deviceGroup);
            Context.ContextChanged();
            return deviceGroup.Guid;
        }

        public bool RemoveDeviceGroup(Guid deviceGroupGuid, DeviceType deviceType)
        {
            var deviceGroups = GetDeviceGroupList(deviceType);
            if (!deviceGroups.Remove(DeviceGroup.FindDeviceGroup(deviceGroups, deviceGroupGuid))) return false;
            Context.ContextChanged();
            return true;
        }

        public bool RenameDeviceGroup(Guid deviceGroupGuid, DeviceType deviceType, string title)
        {
            var deviceGroups = GetDeviceGroupList(deviceType);
            DeviceGroup.FindDeviceGroup(deviceGroups, deviceGroupGuid).Title = title;
            Context.ContextChanged();
            return true;
        }

        public void AddDeviceToDeviceGroup(Device device, DeviceType deviceType, Guid deviceGroupGuid)
        {
            GetDeviceGroupList(deviceType).First(d => d.Guid == deviceGroupGuid).Devices.Add(device);
            Context.ContextChanged();
        }

        public void RemoveDeviceFromDeviceGroup(Device device, DeviceType deviceType, Guid deviceGroupGuid)
        {
            GetDeviceGroup(deviceType, deviceGroupGuid).RemoveDevice(device.Guid);
            Context.ContextChanged();
        }

        public List<DeviceGroup> GetDeviceGroupList(DeviceType deviceType)
        {
            switch (deviceType)
            {
                case DeviceType.Joystick:
                    return JoystickGroups;
                case DeviceType.Keyboard:
                    return KeyboardGroups;
                case DeviceType.Mouse:
                    return MiceGroups;
                case DeviceType.Generic:
                    return GenericDeviceGroups;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deviceType), deviceType, null);
            }
        }
    }
}
