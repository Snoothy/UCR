using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UCR.Models.Devices;
using UCR.Models.Plugins;
using UCR.Models.Mapping;
using UCR.Utilities;
using UCR.ViewModels;

namespace UCR.Models
{
    public class Profile
    {
        public static string GlobalProfileTitle = "Global";

        // Persistence
        public string Title { get; set; }
        public Profile Parent { get; set; }
        public Guid Guid { get; set; }
        public List<Profile> ChildProfiles { get; set; }
        public List<Plugin> Plugins { get; set; }
        // Inputs
        public string KeyboardInputList { get; set; }
        public string MiceInputList { get; set; }
        public string JoystickInputList { get; set; }
        public string GenericInputList { get; set; }
        // Outputs
        public string KeyboardOutputList { get; set; }
        public string MiceOutputList { get; set; }
        public string JoystickOutputList { get; set; }
        public string GenericOutputList { get; set; }

        // Runtime
        public UCRContext ctx;

        private DeviceGroup<Keyboard> InputKeyboards { get; set; }
        private DeviceGroup<Mouse> InputMice { get; set; }
        private DeviceGroup<Device> InputJoysticks { get; set; }
        private DeviceGroup<GenericDevice> InputGenerics { get; set; }

        private DeviceGroup<Keyboard> OutputKeyboards { get; set; }
        private DeviceGroup<Mouse> OutputMice { get; set; }
        private DeviceGroup<Joystick> OutputJoysticks { get; set; }
        private DeviceGroup<GenericDevice> OutputGenerics { get; set; }

        public bool InheritFromParent { get; set; }

        public Profile(UCRContext ctx)
        {
            this.ctx = ctx;
            Plugins = new List<Plugin>();
            InheritFromParent = true;
            Guid = Guid.NewGuid();
        }

        public Profile(UCRContext ctx, Profile parent = null) : this(ctx)
        {
            Parent = parent;
        }

        public bool Activate(UCRContext ctx)
        {
            bool success = true;
            InitializeDeviceGroups(ctx);
            foreach (var plugin in Plugins)
            {
                success &= plugin.Activate(ctx);
            }

            if (Parent != null && InheritFromParent)
            {
                Parent.Activate(ctx);
            }

            return success;
        }

        /// <summary>
        /// Subscribes individual output devices so the active profile can write to them
        /// Used when activating the profile
        /// </summary>
        /// <returns>Returns true if all devices successfully subscribed</returns>
        private bool SubscribeOutputDevices()
        {
            bool success = true;
            success &= OutputJoysticks.Devices.Aggregate(true, (current, device) => current & device.SubscribeOutput(ctx));
            success &= OutputKeyboards.Devices.Aggregate(true, (current, device) => current & device.SubscribeOutput(ctx));
            success &= OutputMice.Devices.Aggregate(true, (current, device) => current & device.SubscribeOutput(ctx));
            success &= OutputGenerics.Devices.Aggregate(true, (current, device) => current & device.SubscribeOutput(ctx));
            return success;
        }

        public void InitializeDeviceGroups(UCRContext ctx, bool reload = false)
        {
            if (!reload && !(InputJoysticks == null || InputKeyboards == null || InputMice == null || InputGenerics == null)) return;
            // TODO Merge with devices from parent profiles
            // TODO Unsubscribe on reload
            // Input
            
            InputJoysticks = new DeviceGroup<Device>()
            {
                GUID = JoystickInputList,
                Devices = DeviceGroup<Joystick>.FindDeviceGroup(ctx.JoystickGroups, JoystickInputList)?.Devices.OfType<Joystick>().Cast<Device>().ToList()
                //Devices = Device.CopyDeviceList<Joystick>(DeviceGroup<Joystick>.FindDeviceGroup(ctx.JoystickGroups, JoystickInputList)?.Devices)
            };

            InputKeyboards = new DeviceGroup<Keyboard>()
            {
                GUID = KeyboardInputList,
                Devices = Device.CopyDeviceList<Keyboard>(DeviceGroup<Keyboard>.FindDeviceGroup(ctx.KeyboardGroups, KeyboardInputList)?.Devices)
            };

            InputMice = new DeviceGroup<Mouse>()
            {
                GUID = MiceInputList,
                Devices = Device.CopyDeviceList<Mouse>(DeviceGroup<Mouse>.FindDeviceGroup(ctx.MiceGroups, MiceInputList)?.Devices)
            };

            InputGenerics = new DeviceGroup<GenericDevice>()
            {
                GUID = GenericInputList,
                Devices = Device.CopyDeviceList<GenericDevice>(DeviceGroup<GenericDevice>.FindDeviceGroup(ctx.GenericDeviceGroups, MiceInputList)?.Devices)
            };

            // Output
            OutputJoysticks = new DeviceGroup<Joystick>()
            {
                GUID = JoystickOutputList,
                Devices = Device.CopyDeviceList<Joystick>(DeviceGroup<Joystick>.FindDeviceGroup(ctx.JoystickGroups, JoystickOutputList)?.Devices)
            };

            OutputKeyboards = new DeviceGroup<Keyboard>()
            {
                GUID = KeyboardOutputList,
                Devices = Device.CopyDeviceList<Keyboard>(DeviceGroup<Keyboard>.FindDeviceGroup(ctx.KeyboardGroups, KeyboardInputList)?.Devices)
            };

            OutputMice = new DeviceGroup<Mouse>()
            {
                GUID = MiceOutputList,
                Devices = Device.CopyDeviceList<Mouse>(DeviceGroup<Mouse>.FindDeviceGroup(ctx.MiceGroups, MiceInputList)?.Devices)
            };

            OutputGenerics = new DeviceGroup<GenericDevice>()
            {
                GUID = GenericOutputList,
                Devices = Device.CopyDeviceList<GenericDevice>(DeviceGroup<GenericDevice>.FindDeviceGroup(ctx.GenericDeviceGroups, MiceInputList)?.Devices)
            };
        }

        public void AddNewChildProfile(string title)
        {
            if (IsGlobalProfileTitle(title)) title += " not allowed";
            if (ChildProfiles == null) ChildProfiles = new List<Profile>();
            ChildProfiles.Add(CreateProfile(ctx, title, this));
        }

        public void Delete()
        {
            if (Parent == null)
            {
                ctx.Profiles.Remove(this);
            }
            else
            {
                Parent.ChildProfiles.Remove(this);
            }
            ctx.IsNotSaved = true;
        }

        public bool Rename(string title)
        {
            if (IsGlobalProfileTitle(title)) return false;
            Title = title;
            return true;
        }
        public static Profile CreateProfile(UCRContext ctx, string title, Profile parent = null)
        {
            if (IsGlobalProfileTitle(title)) title += " not allowed";
            var profile = new Profile(ctx, parent)
            {
                Title = title
            };

            return profile;
        }

        public void SubscribeDeviceLists()
        {

            foreach (var device in InputJoysticks.Devices)
            {
                device.SubscribeDeviceBindings(ctx);
            }
            foreach (var device in InputKeyboards.Devices)
            {
                device.SubscribeDeviceBindings(ctx);
            }
            foreach (var device in InputMice.Devices)
            {
                device.SubscribeDeviceBindings(ctx);
            }
            var success = SubscribeOutputDevices();
        }

        public Device GetInputDevice(DeviceBinding deviceBinding)
        {
            InitializeDeviceGroups(ctx);
            dynamic deviceList;
            switch (deviceBinding.DeviceType)
            {
                case DeviceType.Keyboard:
                    deviceList = InputKeyboards.Devices;
                    break;
                case DeviceType.Mouse:
                    deviceList = InputMice.Devices;
                    break;
                case DeviceType.Joystick:
                    deviceList = InputJoysticks.Devices;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (deviceBinding.DeviceNumber < deviceList.Count)
            {
                return deviceList[deviceBinding.DeviceNumber];
            }
            return null;
        }

        public Device GetOutputDevice(DeviceBinding deviceBinding)
        {
            InitializeDeviceGroups(ctx);
            dynamic deviceList;
            switch (deviceBinding.DeviceType)
            {
                case DeviceType.Keyboard:
                    deviceList = OutputKeyboards.Devices;
                    break;
                case DeviceType.Mouse:
                    deviceList = OutputMice.Devices;
                    break;
                case DeviceType.Joystick:
                    deviceList = OutputJoysticks.Devices;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (deviceBinding.DeviceNumber < deviceList.Count)
            {
                return deviceList[deviceBinding.DeviceNumber];
            }
            return null;
        }

        public void AddPlugin(Plugin plugin)
        {
            plugin.BindingCallback = OnDeviceBindingChange;
            plugin.ParentProfile = this;
            Plugins.Add(plugin);
        }

        public static bool IsGlobalProfileTitle(string title)
        {
            return string.Compare(title, GlobalProfileTitle, StringComparison.InvariantCultureIgnoreCase) == 0;
        }


        private void OnDeviceBindingChange(Plugin plugin)
        {
            if (!IsActive()) return;
            foreach (var deviceBinding in plugin.GetInputs())
            {
                // TODO Resubscribe bindings
                GetInputDevice(deviceBinding).SubscribeDeviceBindingInput(ctx,deviceBinding);
            }
        }

        /// <summary>
        /// Returns true if bindings are currently subscribed to the backend
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return ctx.ActiveProfile.Guid == Guid;
        }

        public List<Device> GetDeviceList(DeviceType? deviceType)
        {
            // TODO Implement
            return new List<Device>();
            throw new NotImplementedException();
        }
    }
}
