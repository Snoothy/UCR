using System;
using System.Collections.Generic;
using System.Linq;
using IOWrapper;
using Providers;
using UCR.Models.Devices;
using UCR.Models.Plugins;
using UCR.Models.Plugins.Remapper;
using UCR.Utilities;

namespace UCR.Models
{
    public class UCRContext
    {
        // Persistence
        public List<Profile> Profiles { get; set; }
        public List<DeviceGroup> KeyboardGroups { get; set; }
        public List<DeviceGroup> MiceGroups { get; set; }
        public List<DeviceGroup> JoystickGroups { get; set; }
        public List<DeviceGroup> GenericDeviceGroups { get; set; }

        // Runtime
        public bool IsNotSaved { get; set; }
        public Profile ActiveProfile { get; set; }
        public IOController IOController { get; set; }

        public UCRContext(bool useMock = true)
        {
            IsNotSaved = false;
            IOController = new IOController();
            Init();
            if (useMock) InitMock();
        }

        private void Init()
        {
            Profiles = new List<Profile>();
            KeyboardGroups = new List<DeviceGroup>();
            MiceGroups = new List<DeviceGroup>();
            JoystickGroups = new List<DeviceGroup>();
            GenericDeviceGroups = new List<DeviceGroup>();
        }


        #region Profile
        
        public bool AddProfile(string title)
        {
            Profiles.Add(Profile.CreateProfile(this, title));
            IsNotSaved = true;
            return true;
        }

        private Profile GetGlobalProfile()
        {
            // TODO Find it properly
            return Profiles.Find(p => p.Title.Equals("Global"));
        }

        public void ActivateProfile(Profile profile)
        {
            bool success = true;
            var lastActiveProfile = ActiveProfile;
            ActiveProfile = profile;
            success &= profile.Activate(this);
            if (success)
            {
                profile.SubscribeDeviceLists();
                IOController.SetProfileState(profile.Guid, true);
                DeactiveProfile(lastActiveProfile);
            }
            else
            {
                // Activation failed, old profile is still active
                ActiveProfile = lastActiveProfile;
            }
        }

        public void DeactiveProfile(Profile profile)
        {
            if (profile == null) return;
            if (ActiveProfile.Guid == profile.Guid) ActiveProfile = null;
            // TODO unsubscribe all outputs and cleanup
        }
        
        #endregion

        #region DeviceGroup

        public DeviceGroup GetDeviceGroup(DeviceType deviceType, Guid deviceGroupGuid)
        {
            return GetDeviceGroupList(deviceType).FirstOrDefault(d => d.Guid == deviceGroupGuid);
        }

        public Guid AddDeviceGroup(string Title, DeviceType deviceType)
        {
            var deviceGroup = new DeviceGroup(Title);
            GetDeviceGroupList(deviceType).Add(deviceGroup);
            IsNotSaved = true;
            return deviceGroup.Guid;
        }

        public bool RemoveDeviceGroup(Guid deviceGroupGuid, DeviceType deviceType)
        {
            var deviceGroups = GetDeviceGroupList(deviceType);
            if (!deviceGroups.Remove(DeviceGroup.FindDeviceGroup(deviceGroups, deviceGroupGuid))) return false;
            IsNotSaved = true;
            return true;
        }

        public bool RenameDeviceGroup(Guid deviceGroupGuid, DeviceType deviceType, string title)
        {
            var deviceGroups = GetDeviceGroupList(deviceType);
            DeviceGroup.FindDeviceGroup(deviceGroups, deviceGroupGuid).Title = title;
            IsNotSaved = true;
            return true;
        }

        public void AddDeviceToDeviceGroup(Device device, DeviceType deviceType, Guid deviceGroupGuid)
        {
            GetDeviceGroupList(deviceType).First(d => d.Guid == deviceGroupGuid).Devices.Add(device);
            IsNotSaved = true;
        }

        public void RemoveDeviceFromDeviceGroup(Device device, DeviceType deviceType, Guid deviceGroupGuid)
        {
            GetDeviceGroup(deviceType, deviceGroupGuid).RemoveDevice(device.Guid);
            IsNotSaved = true;
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

        #endregion

        private void InitMock()
        {
            var inputGuid = Guid.NewGuid();
            var outputGuid = Guid.NewGuid();

            Profiles = new List<Profile>
            {
                new Profile(this)
                {
                    Title = "Global",
                    JoystickInputList = inputGuid,
                    JoystickOutputList = outputGuid
                },
                new Profile(this)
                {
                    Title = "Button Test",
                    JoystickInputList = inputGuid,
                    JoystickOutputList = outputGuid
                },
                new Profile(this)
                {
                    Title = "Axis Test",
                    JoystickInputList = inputGuid,
                    JoystickOutputList = outputGuid
                },
                new Profile(this)
                {
                    Title = "Blank",
                    JoystickInputList = inputGuid,
                    JoystickOutputList = outputGuid
                }

            };

            JoystickGroups = new List<DeviceGroup>()
            {
                new DeviceGroup()
                {
                    Title = "Test input group",
                    Guid = inputGuid
                },
                new DeviceGroup()
                {
                    Title = "vJoy output group",
                    Guid = outputGuid
                }
            };

            var list = IOController.GetInputList();

            foreach (var providerList in list)
            {
                foreach (var device in providerList.Value.Devices)
                {
                    JoystickGroups[0].Devices.Add(new Device(new Guid())
                    {
                        Title = device.Value.DeviceName,
                        DeviceHandle = device.Value.DeviceHandle,
                        SubscriberProviderName = device.Value.ProviderName,
                        SubscriberSubProviderName = device.Value.SubProviderName,
                        Bindings = device.Value.Bindings
                    });
                }
            }

            list = IOController.GetOutputList();

            foreach (var providerList in list)
            {
                foreach (var device in providerList.Value.Devices)
                {
                    JoystickGroups[1].Devices.Add(new Device(new Guid())
                    {
                        Title = device.Value.DeviceName,
                        DeviceHandle = device.Value.DeviceHandle,
                        SubscriberProviderName = device.Value.ProviderName,
                        Bindings = device.Value.Bindings
                    });
                }
            }

            Profile global = GetGlobalProfile();

            for (int i = 0; i < 10; i++)
            {
                Plugin plugin;
                if (i % 2 == 0)
                {
                    plugin = new ButtonToButton()
                    {
                        Title = "ButtonToButton test " + i
                    };
                }
                else
                {
                    plugin = new ButtonToAxis()
                    {
                        Title = "ButtonToAxis test" + i
                    };
                }

                plugin.Inputs[0].DeviceType = (DeviceType)(i%4);
                plugin.Inputs[0].KeyType = (int)InputType.BUTTON;
                plugin.Inputs[0].KeyValue = i;
                plugin.Inputs[0].IsBound = true;

                if (i == 1)
                {
                    plugin.Inputs[0].KeyType = (int)InputType.AXIS;
                }

                plugin.Outputs[0].DeviceType = DeviceType.Joystick;
                plugin.Outputs[0].KeyType = (int)InputType.BUTTON;
                plugin.Outputs[0].KeyValue = i;
                plugin.Outputs[0].IsBound = true;

                global.AddPlugin(plugin);
            }


            Profiles[1].AddPlugin(new ButtonToButton()
            {
                Title = "Button test"
            });

            Profiles[2].AddPlugin(new ButtonToAxis()
            {
                Title = "Axis test"
            });
        }
    }
}
