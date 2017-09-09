using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
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
        private static string GlobalProfileTitle = "Global";

        /* Persistence */
        public string Title { get; set; }
        public Profile Parent { get; set; }
        public Guid Guid { get; set; }
        public List<Profile> ChildProfiles { get; set; }
        public List<Plugin> Plugins { get; set; }

        // Inputs
        public Guid KeyboardInputList { get; set; }
        public Guid MiceInputList { get; set; }
        public Guid JoystickInputList { get; set; }
        public Guid GenericInputList { get; set; }

        // Outputs
        public Guid KeyboardOutputList { get; set; }
        public Guid MiceOutputList { get; set; }
        public Guid JoystickOutputList { get; set; }
        public Guid GenericOutputList { get; set; }

        /* Runtime */
        public UCRContext ctx;
        private Dictionary<DeviceType, DeviceGroup> InputGroups { get; set; }
        private Dictionary<DeviceType, DeviceGroup> OutputGroups { get; set; }
        private Dictionary<DeviceBindingType, Dictionary<DeviceType, Guid>> DeviceGroupGuids { get; set; }

        public bool InheritFromParent { get; set; }

        #region Constructors

        public Profile(UCRContext ctx)
        {
            this.ctx = ctx;
            Plugins = new List<Plugin>();
            InheritFromParent = true;
            Guid = Guid.NewGuid();

            ChildProfiles = new List<Profile>();
            Plugins = new List<Plugin>();

            InputGroups = new Dictionary<DeviceType, DeviceGroup>();
            OutputGroups = new Dictionary<DeviceType, DeviceGroup>();

            SetDeviceListNames();
        }

        public Profile(UCRContext ctx, Profile parent = null) : this(ctx)
        {
            Parent = parent;
        }

        #endregion

        #region Actions

        public void AddNewChildProfile(string title)
        {
            if (IsGlobalProfileTitle(title)) title += " not allowed";
            if (ChildProfiles == null) ChildProfiles = new List<Profile>();
            ChildProfiles.Add(CreateProfile(ctx, title, this));
        }

        public bool Rename(string title)
        {
            if (IsGlobalProfileTitle(title)) return false;
            Title = title;
            return true;
        }

        public void Remove()
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

        public void SetDeviceGroup(DeviceBindingType bindingType, DeviceType deviceType, Guid deviceGroupGuid)
        {
            switch (bindingType)
            {
                case DeviceBindingType.Input:
                    switch (deviceType)
                    {
                        case DeviceType.Joystick:
                            JoystickInputList = deviceGroupGuid;
                            break;
                        case DeviceType.Keyboard:
                            KeyboardInputList = deviceGroupGuid;
                            break;
                        case DeviceType.Mouse:
                            MiceInputList = deviceGroupGuid;
                            break;
                        case DeviceType.Generic:
                            GenericInputList = deviceGroupGuid;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(deviceType), deviceType, null);
                    }
                    break;
                case DeviceBindingType.Output:
                    switch (deviceType)
                    {
                        case DeviceType.Joystick:
                            JoystickOutputList = deviceGroupGuid;
                            break;
                        case DeviceType.Keyboard:
                            KeyboardOutputList = deviceGroupGuid;
                            break;
                        case DeviceType.Mouse:
                            MiceOutputList = deviceGroupGuid;
                            break;
                        case DeviceType.Generic:
                            GenericOutputList = deviceGroupGuid;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(deviceType), deviceType, null);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bindingType), bindingType, null);
            }
            SetDeviceListNames();
            ctx.IsNotSaved = true;
        }

        #endregion

        #region Subscription
        
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

        // TODO always start from a clean slate
        private void InitializeDeviceGroups(bool reload = false)
        {
            if (!reload)
            {
                reload = Enum.GetValues(typeof(DeviceType)).Cast<object>().Aggregate(reload,
                    (current, type) => current || !InputGroups.ContainsKey((DeviceType)type));
                if (!reload) return;
            }
            SetDeviceListNames();
            // TODO Merge with devices from parent profiles
            // TODO Unsubscribe on reload

            // Input
            foreach (var type in Enum.GetValues(typeof(DeviceType)))
            {
                InputGroups[(DeviceType)type] = new DeviceGroup()
                {
                    Guid = DeviceGroupGuids[DeviceBindingType.Input][(DeviceType)type],
                    Devices = GetCopiedList((DeviceType)type,
                        DeviceGroupGuids[DeviceBindingType.Input][(DeviceType)type])
                };
            }

            // Output
            foreach (var type in Enum.GetValues(typeof(DeviceType)))
            {
                OutputGroups[(DeviceType)type] = new DeviceGroup()
                {
                    Guid = DeviceGroupGuids[DeviceBindingType.Output][(DeviceType)type],
                    Devices = GetCopiedList((DeviceType)type,
                        DeviceGroupGuids[DeviceBindingType.Output][(DeviceType)type])
                };
            }
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
                success &= OutputGroups[(DeviceType)type].Devices
                    .Aggregate(true, (current, device) => current & device.SubscribeOutput(ctx));
            }
            return success;
        }

        #endregion

        #region Device

        public Guid GetDeviceListGuid(DeviceBinding deviceBinding)
        {
            SetDeviceListNames();
            return DeviceGroupGuids[deviceBinding.DeviceBindingType][deviceBinding.DeviceType];
        }

        public Device GetDevice(DeviceBinding deviceBinding)
        {
            var deviceList = GetDeviceList(deviceBinding);
            return deviceBinding.DeviceNumber < deviceList.Count
                ? deviceList[deviceBinding.DeviceNumber]
                : null;
        }

        public List<Device> GetDeviceList(DeviceBinding deviceBinding)
        {
            return ctx.GetDeviceGroup(deviceBinding.DeviceType, GetDeviceListGuid(deviceBinding))?.Devices ?? new List<Device>();
        }

        public Device GetLocalDevice(DeviceBinding deviceBinding)
        {
            var deviceList = GetLocalDeviceList(deviceBinding);
            return deviceBinding.DeviceNumber < deviceList.Count ? deviceList[deviceBinding.DeviceNumber] : null;
        }

        private List<Device> GetLocalDeviceList(DeviceBinding deviceBinding)
        {
            var deviceGroups = deviceBinding.DeviceBindingType == DeviceBindingType.Input ? InputGroups : OutputGroups;
            var deviceList = deviceGroups[deviceBinding.DeviceType].Devices;
            return deviceList;
        }

        #endregion
        
        #region Plugin

        public void AddPlugin(Plugin plugin, string title = "Untitled")
        {
            if (plugin.Title == null) plugin.Title = title;
            plugin.BindingCallback = OnDeviceBindingChange;
            plugin.ParentProfile = this;
            Plugins.Add(plugin);
        }

        public void RemovePlugin(Plugin plugin)
        {
            Plugins.Remove(plugin);
            ctx.IsNotSaved = true;
        }

        #endregion
        
        #region Helpers

        /// <summary>
        /// Returns true if bindings are currently subscribed to the backend
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return ctx.ActiveProfile != null && ctx.ActiveProfile.Guid == Guid;
        }

        private static bool IsGlobalProfileTitle(string title)
        {
            return string.Compare(title, GlobalProfileTitle, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        private static List<Device> CopyDeviceList(List<DeviceGroup> group, Guid groupGuid)
        {
            return Device.CopyDeviceList(DeviceGroup.FindDeviceGroup(group, groupGuid)?.Devices);
        }

        private List<Device> GetCopiedList(DeviceType deviceType, Guid groupGuid)
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

        private void SetDeviceListNames()
        {
            DeviceGroupGuids = new Dictionary<DeviceBindingType, Dictionary<DeviceType, Guid>>()
            {
                {
                    DeviceBindingType.Input, new Dictionary<DeviceType, Guid>()
                    {
                        {DeviceType.Generic, GenericInputList},
                        {DeviceType.Joystick, JoystickInputList},
                        {DeviceType.Keyboard, KeyboardInputList},
                        {DeviceType.Mouse, MiceInputList}
                    }
                },
                {
                    DeviceBindingType.Output, new Dictionary<DeviceType, Guid>()
                    {
                        {DeviceType.Generic, GenericOutputList},
                        {DeviceType.Joystick, JoystickOutputList},
                        {DeviceType.Keyboard, KeyboardOutputList},
                        {DeviceType.Mouse, MiceOutputList}
                    }
                }
            };
        }

        #endregion

        public static Profile CreateProfile(UCRContext ctx, string title, Profile parent = null)
        {
            if (IsGlobalProfileTitle(title)) title += " not allowed";
            var profile = new Profile(ctx, parent)
            {
                Title = title
            };

            return profile;
        }

        private void OnDeviceBindingChange(Plugin plugin)
        {
            if (!IsActive()) return;
            foreach (var deviceBinding in plugin.GetInputs())
            {
                // TODO Resubscribe bindings
                GetLocalDevice(deviceBinding).SubscribeDeviceBindingInput(ctx, deviceBinding);
            }
        }
    }
}