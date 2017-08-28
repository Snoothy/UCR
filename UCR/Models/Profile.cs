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

        private Dictionary<DeviceType, DeviceGroup<Device>> InputGroups { get; set; }
        private Dictionary<DeviceType, DeviceGroup<Device>> OutputGroups { get; set; }
        private Dictionary<DeviceBindingType, Dictionary<DeviceType, string>> DeviceListNames { get; set; }

        public bool InheritFromParent { get; set; }

        public Profile(UCRContext ctx)
        {
            this.ctx = ctx;
            Plugins = new List<Plugin>();
            InheritFromParent = true;
            Guid = Guid.NewGuid();

            InputGroups = new Dictionary<DeviceType, DeviceGroup<Device>>();
            OutputGroups = new Dictionary<DeviceType, DeviceGroup<Device>>();

            SetDeviceListNames();
        }

        private void SetDeviceListNames()
        {
            DeviceListNames = new Dictionary<DeviceBindingType, Dictionary<DeviceType, string>>()
            {
                {DeviceBindingType.Input, new Dictionary<DeviceType, string>()
                {
                    {DeviceType.Generic, GenericInputList},
                    {DeviceType.Joystick, JoystickInputList},
                    {DeviceType.Keyboard, KeyboardInputList},
                    {DeviceType.Mouse, MiceInputList}
                } },
                {DeviceBindingType.Output, new Dictionary<DeviceType, string>()
                {
                    {DeviceType.Generic, GenericOutputList},
                    {DeviceType.Joystick, JoystickOutputList},
                    {DeviceType.Keyboard, KeyboardOutputList},
                    {DeviceType.Mouse, MiceOutputList}
                } }
            };
        }

        public Profile(UCRContext ctx, Profile parent = null) : this(ctx)
        {
            Parent = parent;
        }

        public bool Activate(UCRContext ctx)
        {
            bool success = true;
            InitializeDeviceGroups();
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
            foreach (var type in Enum.GetValues(typeof(DeviceType)))
            {
                success &= OutputGroups[(DeviceType)type].Devices.Aggregate(true, (current, device) => current & device.SubscribeOutput(ctx));
            }
            return success;
        }

        private static List<Device> CopyDeviceList(List<DeviceGroup<Device>> group, string groupGuid)
        {
            return Device.CopyDeviceList(DeviceGroup<Device>.FindDeviceGroup(group, groupGuid)?.Devices);
        }

        private List<Device> GetCopiedList(DeviceType deviceType, string groupGuid)
        {
            switch (deviceType)
            {
                case DeviceType.Keyboard:
                    return CopyDeviceList(ctx.KeyboardGroups, groupGuid);
                case DeviceType.Mouse:
                    return CopyDeviceList(ctx.MiceGroups, groupGuid);
                case DeviceType.Joystick:
                    return CopyDeviceList(ctx.JoystickGroups, groupGuid);
                case DeviceType.Generic:
                    return CopyDeviceList(ctx.GenericDeviceGroups, groupGuid);
                default:
                    throw new ArgumentOutOfRangeException(nameof(deviceType), deviceType, null);
            }
        }

        public void InitializeDeviceGroups(bool reload = false)
        {
            if (!reload)
            {
                reload = Enum.GetValues(typeof(DeviceType)).Cast<object>().Aggregate(reload, (current, type) => current || !InputGroups.ContainsKey((DeviceType) type));
                if (!reload) return;
            }
            SetDeviceListNames();
            // TODO Merge with devices from parent profiles
            // TODO Unsubscribe on reload
            
            // Input
            foreach (var type in Enum.GetValues(typeof(DeviceType)))
            {
                InputGroups[(DeviceType) type] = new DeviceGroup<Device>()
                {
                    GUID = DeviceListNames[DeviceBindingType.Input][(DeviceType)type],
                    Devices = GetCopiedList((DeviceType)type, DeviceListNames[DeviceBindingType.Input][(DeviceType)type])
                };
            }

            // Output
            foreach (var type in Enum.GetValues(typeof(DeviceType)))
            {
                OutputGroups[(DeviceType)type] = new DeviceGroup<Device>()
                {
                    GUID = DeviceListNames[DeviceBindingType.Output][(DeviceType)type],
                    Devices = GetCopiedList((DeviceType)type, DeviceListNames[DeviceBindingType.Output][(DeviceType)type])
                };
            }
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
            foreach (var deviceGroup in InputGroups)
            {
                foreach (var device in deviceGroup.Value.Devices)
                {
                    device.SubscribeDeviceBindings(ctx);
                }
            }
            var success = SubscribeOutputDevices();
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
                GetDevice(deviceBinding).SubscribeDeviceBindingInput(ctx,deviceBinding);
            }
        }

        /// <summary>
        /// Returns true if bindings are currently subscribed to the backend
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return ctx.ActiveProfile != null && ctx.ActiveProfile.Guid == Guid;
        }


        public Device GetDevice(DeviceBinding deviceBinding)
        {
            var deviceList = GetDeviceList(deviceBinding);
            return deviceBinding.DeviceNumber < deviceList.Count ? deviceList[deviceBinding.DeviceNumber] : null;
        }

        public List<Device> GetDeviceList(DeviceBinding deviceBinding)
        {
            InitializeDeviceGroups();
            var deviceGroups = deviceBinding.DeviceBindingType == DeviceBindingType.Input ? InputGroups : OutputGroups;
            var deviceList = deviceGroups[deviceBinding.DeviceType].Devices;
            return deviceList;
        }
    }
}
