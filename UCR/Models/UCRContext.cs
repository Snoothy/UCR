using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using IOWrapper;
using Providers;
using UCR.Models.Devices;
using UCR.Models.Plugins;
using UCR.Models.Plugins.Remapper;

namespace UCR.Models
{
    public class UCRContext
    {
        // Persistence
        public List<Profile> Profiles { get; set; }
        public List<DeviceGroup<Device>> KeyboardGroups { get; set; }
        public List<DeviceGroup<Device>> MiceGroups { get; set; }
        public List<DeviceGroup<Device>> JoystickGroups { get; set; }
        public List<DeviceGroup<Device>> GenericDeviceGroups { get; set; }

        // Runtime
        public bool IsNotSaved { get; set; }
        public Profile ActiveProfile { get; set; }
        public IOController IOController { get; set; }

        public UCRContext()
        {
            IsNotSaved = false;
            IOController = new IOController();
            Init();
        }

        private void Init()
        {
            KeyboardGroups = new List<DeviceGroup<Device>>();
            MiceGroups = new List<DeviceGroup<Device>>();
            JoystickGroups = new List<DeviceGroup<Device>>();
            GenericDeviceGroups = new List<DeviceGroup<Device>>();
            InitMock();
        }

        public void ActivateProfile(Profile profile)
        {
            bool success = true;
            var lastActiveProfile = ActiveProfile;
            ActiveProfile = profile;
            success &= profile.Activate(this);
            if (success)
            {
                ActiveProfile.SubscribeDeviceLists();
                IOController.SetProfileState(ActiveProfile.Guid, true);
            }
            else
            {
                // Activation failed, old profile is still active
                ActiveProfile = lastActiveProfile;
            }
        }

        private Profile GetGlobalProfile()
        {
            // TODO Find it properly
            return Profiles.Find(p => p.Title.Equals("Global"));
        }

        private void InitMock()
        {
            Profiles = new List<Profile>
            {
                new Profile(this)
                {
                    Title = "Global",
                    JoystickInputList = "FAKEGUID",
                    JoystickOutputList = "FAKEGUIDOUTPUT"
                },
                new Profile(this)
                {
                    Title = "Button Test",
                    JoystickInputList = "FAKEGUID",
                    JoystickOutputList = "FAKEGUIDOUTPUT"
                },
                new Profile(this)
                {
                    Title = "Axis Test",
                    JoystickInputList = "FAKEGUID",
                    JoystickOutputList = "FAKEGUIDOUTPUT"
                }
            };

            JoystickGroups = new List<DeviceGroup<Device>>()
            {
                new DeviceGroup<Device>()
                {
                    GUID = "FAKEGUID"
                },
                new DeviceGroup<Device>()
                {
                    GUID = "FAKEGUIDOUTPUT"
                }
            };

            var list = IOController.GetInputList();

            foreach (var providerList in list)
            {
                foreach (var device in providerList.Value.Devices)
                {
                    JoystickGroups[0].Devices.Add(new Device()
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
                    JoystickGroups[1].Devices.Add(new Device()
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
