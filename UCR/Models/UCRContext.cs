using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using IOWrapper;
using UCR.Models.Devices;
using UCR.Models.Plugins;
using UCR.Models.Plugins.Remapper;

namespace UCR.Models
{
    public class UCRContext
    {
        // Persistence
        public List<Profile> Profiles { get; set; }
        public List<DeviceGroup<Keyboard>> KeyboardGroups { get; set; }
        public List<DeviceGroup<Mouse>> MiceGroups { get; set; }
        public List<DeviceGroup<Joystick>> JoystickGroups { get; set; }
        public List<DeviceGroup<GenericDevice>> GenericDeviceGroups { get; set; }

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

        public void Init()
        {
            KeyboardGroups = new List<DeviceGroup<Keyboard>>();
            MiceGroups = new List<DeviceGroup<Mouse>>();
            JoystickGroups = new List<DeviceGroup<Joystick>>();
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
                    Title = "N64"
                }
            };

            JoystickGroups = new List<DeviceGroup<Joystick>>()
            {
                new DeviceGroup<Joystick>()
                {
                    GUID = "FAKEGUID"
                },
                new DeviceGroup<Joystick>()
                {
                    GUID = "FAKEGUIDOUTPUT"
                }
            };
            JoystickGroups[0].Devices.Add(new Joystick()
            {
                Title = "Joystick mock name",
                DeviceHandle = "JOYSTICKGUID"
            });

            // Output
            JoystickGroups[1].Devices.Add(new Joystick()
            {
                Title = "Joystick mock name",
                DeviceHandle = "JOYSTICKGUIDOUTPUT"
            });


            var list = IOController.GetInputList();
            string deviceHandle = null;

            foreach (var providerList in list)
            {
                foreach (var device in providerList.Value.Devices)
                {
                    if (device.Value.ProviderName == "SharpDX_DirectInput" && device.Value.DeviceName == "usb gamepad           ")
                    {
                        JoystickGroups[0].Devices[0].DeviceHandle = device.Value.DeviceHandle;
                        JoystickGroups[0].Devices[0].SubscriberProviderName = device.Value.ProviderName;
                        JoystickGroups[0].Devices[0].MaxButtons = device.Value.ButtonList.Count;
                        break;
                    }
                }
            }

            list = IOController.GetOutputList();

            foreach (var providerList in list)
            {
                foreach (var device in providerList.Value.Devices)
                {
                    if (device.Value.ProviderName == "Core_vJoyInterfaceWrap")
                    {
                        JoystickGroups[1].Devices[0].DeviceHandle = device.Value.DeviceHandle;
                        JoystickGroups[1].Devices[0].SubscriberProviderName = device.Value.ProviderName;
                        JoystickGroups[1].Devices[0].MaxButtons = device.Value.ButtonList.Count;
                        break;
                    }
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

                plugin.Inputs[0].DeviceType = DeviceType.Joystick;
                plugin.Inputs[0].KeyType = (int)KeyType.Button;
                plugin.Inputs[0].KeyValue = i;

                if (i == 1)
                {
                    plugin.Inputs[0].KeyType = (int)KeyType.Axis;
                }

                plugin.Outputs[0].DeviceType = DeviceType.Joystick;
                plugin.Outputs[0].KeyType = (int)KeyType.Button;
                plugin.Outputs[0].KeyValue = i;

                global.AddPlugin(plugin);
            }

        }
    }
}
