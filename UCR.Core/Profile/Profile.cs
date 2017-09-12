using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UCR.Core.Device;

namespace UCR.Core.Profile
{
    public class Profile
    {
        private static string GlobalProfileTitle = "Global";

        /* Persistence */
        public string Title { get; set; }
        public Guid Guid { get; set; }
        public List<Profile> ChildProfiles { get; set; }
        public List<Plugin.Plugin> Plugins { get; set; }

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
        [XmlIgnore]
        public UCRContext ctx;
        [XmlIgnore]
        public Profile ParentProfile { get; set; }
        [XmlIgnore]
        private Dictionary<DeviceType, DeviceGroup> InputGroups { get; set; }
        [XmlIgnore]
        private Dictionary<DeviceType, DeviceGroup> OutputGroups { get; set; }
        [XmlIgnore]
        private Dictionary<DeviceBindingType, Dictionary<DeviceType, Guid>> DeviceGroupGuids { get; set; }
        [XmlIgnore]
        public bool InheritFromParent { get; set; }

        #region Constructors

        private Profile()
        {
            Init();
        }

        public Profile(UCRContext ctx)
        {
            this.ctx = ctx;
            Init();
        }

        private void Init()
        {
            Plugins = Plugins ?? new List<Plugin.Plugin>();
            ChildProfiles = ChildProfiles ?? new List<Profile>();
            Plugins = Plugins ?? new List<Plugin.Plugin>();

            InheritFromParent = true;
            Guid = Guid == Guid.Empty ? Guid.NewGuid() : Guid;
            InputGroups = new Dictionary<DeviceType, DeviceGroup>();
            OutputGroups = new Dictionary<DeviceType, DeviceGroup>();

            SetDeviceListNames();
        }

        public Profile(UCRContext ctx, Profile parentProfile = null) : this(ctx)
        {
            ParentProfile = parentProfile;
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
            if (ParentProfile == null)
            {
                ctx.Profiles.Remove(this);
            }
            else
            {
                ParentProfile.ChildProfiles.Remove(this);
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

            if (ParentProfile != null && InheritFromParent)
            {
                ParentProfile.Activate(ctx);
            }

            foreach (var plugin in Plugins)
            {
                success &= plugin.Activate(ctx);
            }

            return success;
        }

        private void InitializeDeviceGroups()
        {
            SetDeviceListNames();

            // Input
            foreach (var type in Enum.GetValues(typeof(DeviceType)))
            {
                InputGroups[(DeviceType)type] = new DeviceGroup()
                {
                    Guid = DeviceGroupGuids[DeviceBindingType.Input][(DeviceType)type],
                    Devices = GetCopiedList(DeviceBindingType.Input,(DeviceType)type)
                };
            }

            // Output
            foreach (var type in Enum.GetValues(typeof(DeviceType)))
            {
                OutputGroups[(DeviceType)type] = new DeviceGroup()
                {
                    Guid = DeviceGroupGuids[DeviceBindingType.Output][(DeviceType)type],
                    Devices = GetCopiedList(DeviceBindingType.Output, (DeviceType)type)
                };
            }
        }

        public bool SubscribeDeviceLists()
        {
            var success = true;
            foreach (var deviceGroup in InputGroups)
            {
                foreach (var device in deviceGroup.Value.Devices)
                {
                    success &= device.SubscribeDeviceBindings(ctx);
                }
            }
            success &= SubscribeOutputDevices();
            return success;
        }

        public bool UnsubscribeDeviceLists()
        {
            var success = true;
            foreach (var deviceGroup in InputGroups)
            {
                foreach (var device in deviceGroup.Value.Devices)
                {
                    success &= device.UnsubscribeDeviceBindings(ctx);
                }
            }
            success &= UnsubscribeOutputDevices();
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
                success &= OutputGroups[(DeviceType)type].Devices
                    .Aggregate(true, (current, device) => current & device.SubscribeOutput(ctx));
            }
            return success;
        }

        private bool UnsubscribeOutputDevices()
        {
            bool success = true;
            foreach (var type in Enum.GetValues(typeof(DeviceType)))
            {
                success &= OutputGroups[(DeviceType)type].Devices
                    .Aggregate(true, (current, device) => current & device.UnsubscribeOutput(ctx));
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

        public Device.Device GetDevice(DeviceBinding deviceBinding)
        {
            var deviceList = GetDeviceList(deviceBinding);
            return deviceBinding.DeviceNumber < deviceList.Count
                ? deviceList[deviceBinding.DeviceNumber]
                : null;
        }

        public List<Device.Device> GetDeviceList(DeviceBinding deviceBinding)
        {
            return GetDeviceList(deviceBinding.DeviceBindingType, deviceBinding.DeviceType);
        }

        public List<Device.Device> GetDeviceList(DeviceBindingType deviceBindingType, DeviceType deviceType)
        {
            SetDeviceListNames();
            var result = ctx.GetDeviceGroup(deviceType, DeviceGroupGuids[deviceBindingType][deviceType])?.Devices ?? new List<Device.Device>();
            if (!InheritFromParent || ParentProfile == null) return result;

            var parentDeviceList = ParentProfile.GetDeviceList(deviceBindingType, deviceType);
            if (result.Count < parentDeviceList.Count)
            {
                for (var i = result.Count; i < parentDeviceList.Count; i++)
                {
                    result.Add(parentDeviceList[i]);
                }
            }
            return result;
        }

        public Device.Device GetLocalDevice(DeviceBinding deviceBinding)
        {
            var deviceList = GetLocalDeviceList(deviceBinding);
            return deviceBinding.DeviceNumber < deviceList.Count ? deviceList[deviceBinding.DeviceNumber] : null;
        }

        private List<Device.Device> GetLocalDeviceList(DeviceBinding deviceBinding)
        {
            var deviceGroups = deviceBinding.DeviceBindingType == DeviceBindingType.Input ? InputGroups : OutputGroups;
            var deviceList = deviceGroups[deviceBinding.DeviceType].Devices;
            return deviceList;
        }

        #endregion
        
        #region Plugin

        public void AddPlugin(Plugin.Plugin plugin, string title = "Untitled")
        {
            if (plugin.Title == null) plugin.Title = title;
            plugin.BindingCallback = OnDeviceBindingChange;
            plugin.ParentProfile = this;
            Plugins.Add(plugin);
        }

        public void RemovePlugin(Plugin.Plugin plugin)
        {
            Plugins.Remove(plugin);
            ctx.IsNotSaved = true;
        }

        #endregion
        
        #region Helpers

        public string ProfileBreadCrumbs()
        {
            return ParentProfile != null ? ParentProfile.ProfileBreadCrumbs() + " > " + Title : Title;
        }

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

        private List<Device.Device> GetCopiedList(DeviceBindingType deviceBindingType, DeviceType deviceType)
        {
            return Device.Device.CopyDeviceList(GetDeviceList(deviceBindingType, deviceType));
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

        internal void OnDeviceBindingChange(Plugin.Plugin plugin)
        {
            if (!IsActive()) return;
            foreach (var deviceBinding in plugin.GetInputs())
            {
                var device = GetLocalDevice(deviceBinding);
                if (device == null) return; 
                device.SubscribeDeviceBindingInput(ctx, deviceBinding);
            }
        }

        internal void PostLoad(UCRContext ctx, Profile parentProfile = null)
        {
            this.ctx = ctx;
            ParentProfile = parentProfile;

            foreach (var profile in ChildProfiles)
            {
                profile.PostLoad(ctx, this);
            }

            foreach (var plugin in Plugins)
            {
                plugin.PostLoad(ctx, this);
            }
        }
    }
}