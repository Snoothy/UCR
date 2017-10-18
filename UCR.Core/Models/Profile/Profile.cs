using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NLog;
using UCR.Core.Models.Binding;
using UCR.Core.Models.Device;
using UCR.Core.Models.Plugin;

namespace UCR.Core.Models.Profile
{
    public class Profile
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string GlobalProfileTitle = "Global";

        /* Persistence */
        public string Title { get; set; }
        public Guid Guid { get; set; }
        public List<Profile> ChildProfiles { get; set; }
        public List<Models.Plugin.Plugin> Plugins { get; set; }

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
        public Context context;
        [XmlIgnore]
        public Profile ParentProfile { get; set; }
        [XmlIgnore]
        private Dictionary<DeviceType, DeviceGroup> InputGroups { get; set; }
        [XmlIgnore]
        private Dictionary<DeviceType, DeviceGroup> OutputGroups { get; set; }
        [XmlIgnore]
        private Dictionary<DeviceIoType, Dictionary<DeviceType, Guid>> DeviceGroupGuids { get; set; }
        [XmlIgnore]
        public bool InheritFromParent { get; set; }

        #region Constructors

        public Profile()
        {
            Init();
        }

        public Profile(Context context)
        {
            this.context = context;
            Init();
        }

        private void Init()
        {
            Plugins = Plugins ?? new List<Models.Plugin.Plugin>();
            ChildProfiles = ChildProfiles ?? new List<Profile>();
            Plugins = Plugins ?? new List<Models.Plugin.Plugin>();

            InheritFromParent = true;
            Guid = Guid == Guid.Empty ? Guid.NewGuid() : Guid;
            InputGroups = new Dictionary<DeviceType, DeviceGroup>();
            OutputGroups = new Dictionary<DeviceType, DeviceGroup>();

            SetDeviceListNames();
        }

        public Profile(Context context, Profile parentProfile = null) : this(context)
        {
            ParentProfile = parentProfile;
        }

        #endregion

        #region Actions

        public void AddNewChildProfile(string title)
        {
            if (IsGlobalProfileTitle(title)) title += " not allowed";
            if (ChildProfiles == null) ChildProfiles = new List<Profile>();
            ChildProfiles.Add(CreateProfile(context, title, this));
            context.ContextChanged();
        }

        public void AddChildProfile(Profile profile, string title)
        {
            if (IsGlobalProfileTitle(title)) title += " not allowed";
            if (ChildProfiles == null) ChildProfiles = new List<Profile>();
            profile.context = context;
            profile.ParentProfile = this;
            ChildProfiles.Add(profile);
            context.ContextChanged();
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
                context.Profiles.Remove(this);
            }
            else
            {
                ParentProfile.ChildProfiles.Remove(this);
            }
            context.ContextChanged();
        }

        public void SetDeviceGroup(DeviceIoType ioType, DeviceType deviceType, Guid deviceGroupGuid)
        {
            switch (ioType)
            {
                case DeviceIoType.Input:
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
                case DeviceIoType.Output:
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
                    throw new ArgumentOutOfRangeException(nameof(ioType), ioType, null);
            }
            SetDeviceListNames();
            context.ContextChanged();
        }

        public bool Activate()
        {
            return context.ProfilesManager.ActivateProfile(this);
        }

        public bool Deactivate()
        {
            return context.ProfilesManager.DeactivateProfile(this);
        }

        #endregion

        #region Subscription

        public bool Activate(Profile profile)
        {
            var success = true;
            
            if (profile.Guid == Guid) InitializeDeviceGroups();

            if (ParentProfile != null && InheritFromParent)
            {
                Logger.Debug($"Activating parent profile: {{{ParentProfile.Title}}}");
                success &= ParentProfile.Activate(profile);
            }

            foreach (var plugin in Plugins)
            {
                success &= plugin.Activate(profile);
            }

            if (!success) Logger.Error($"Failed to activate \"{{{Title}}}\" when during activation of \"{{{profile.Title}}}\"");

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
                    Guid = DeviceGroupGuids[DeviceIoType.Input][(DeviceType)type],
                    Devices = GetCopiedList(DeviceIoType.Input,(DeviceType)type)
                };
            }

            // Output
            foreach (var type in Enum.GetValues(typeof(DeviceType)))
            {
                OutputGroups[(DeviceType)type] = new DeviceGroup()
                {
                    Guid = DeviceGroupGuids[DeviceIoType.Output][(DeviceType)type],
                    Devices = GetCopiedList(DeviceIoType.Output, (DeviceType)type)
                };
            }
        }

        public bool SubscribeDeviceLists()
        {
            Logger.Debug("Subscribing device lists");
            var success = true;
            foreach (var deviceGroup in InputGroups)
            {
                foreach (var device in deviceGroup.Value.Devices)
                {
                    success &= device.SubscribeDeviceBindings(context);
                }
            }
            success &= SubscribeOutputDevices();
            if (!success) Logger.Error($"Failed to subscribe device lists for: {{{ProfileBreadCrumbs()}}}");
            return success;
        }

        public bool UnsubscribeDeviceLists()
        {
            Logger.Debug("Unsubscribing device lists");
            var success = true;
            foreach (var deviceGroup in InputGroups)
            {
                foreach (var device in deviceGroup.Value.Devices)
                {
                    success &= device.UnsubscribeDeviceBindings(context);
                }
            }
            success &= UnsubscribeOutputDevices();
            if (!success) Logger.Error($"Failed to unsubscribe device lists for: {{{ProfileBreadCrumbs()}}}");
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
                foreach (var device in OutputGroups[(DeviceType)type].Devices)
                {
                    var deviceSuccess = device.SubscribeOutput(context);
                    if (!deviceSuccess) Logger.Error($"Failed to subscribe output device: {{{device.LogName()}}}");
                    success &= deviceSuccess;
                }
            }
            return success;
        }

        private bool UnsubscribeOutputDevices()
        {
            var success = true;
            foreach (var type in Enum.GetValues(typeof(DeviceType)))
            {
                foreach (var device in OutputGroups[(DeviceType)type].Devices)
                {
                    var deviceSuccess = device.UnsubscribeOutput(context);
                    if (!deviceSuccess) Logger.Error($"Failed to unsubscribe output device: {{{device.LogName()}}}");
                    success &= deviceSuccess;
                }
            }
            return success;
        }

        #endregion

        #region Device

        public Guid GetDeviceListGuid(DeviceBinding deviceBinding)
        {
            SetDeviceListNames();
            return DeviceGroupGuids[deviceBinding.DeviceIoType][deviceBinding.DeviceType];
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
            return GetDeviceList(deviceBinding.DeviceIoType, deviceBinding.DeviceType);
        }

        public List<Device.Device> GetDeviceList(DeviceIoType deviceIoType, DeviceType deviceType)
        {
            SetDeviceListNames();
            var result = context.DeviceGroupsManager.GetDeviceGroup(deviceType, DeviceGroupGuids[deviceIoType][deviceType])?.Devices ?? new List<Device.Device>();
            if (!InheritFromParent || ParentProfile == null) return result;

            var parentDeviceList = ParentProfile.GetDeviceList(deviceIoType, deviceType);
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
            var deviceGroups = deviceBinding.DeviceIoType == DeviceIoType.Input ? InputGroups : OutputGroups;
            var deviceList = deviceGroups[deviceBinding.DeviceType].Devices;
            return deviceList;
        }

        #endregion
        
        #region Plugin

        public void AddNewPlugin(Plugin.Plugin plugin, string title = "Untitled")
        {
            AddPlugin((Plugin.Plugin)Activator.CreateInstance(plugin.GetType()), title);
        }

        public void AddPlugin(Plugin.Plugin plugin, string title = "Untitled")
        {
            
            if (plugin.Title == null) plugin.Title = title;
            plugin.BindingCallback = OnDeviceBindingChange;
            plugin.ParentProfile = this;
            plugin.ContainingList = Plugins;
            Plugins.Add(plugin);
            context.ContextChanged();
        }

        public void RemovePlugin(Models.Plugin.Plugin plugin)
        {
            Plugins.Remove(plugin);
            context.ContextChanged();
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
            return context.ActiveProfile != null && context.ActiveProfile.Guid == Guid;
        }

        private static bool IsGlobalProfileTitle(string title)
        {
            return string.Compare(title, GlobalProfileTitle, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        private List<Device.Device> GetCopiedList(DeviceIoType deviceIoType, DeviceType deviceType)
        {
            var deviceList = Device.Device.CopyDeviceList(GetDeviceList(deviceIoType, deviceType));
            deviceList.ForEach(d => d.Reset(this));
            return deviceList;
        }

        private void SetDeviceListNames()
        {
            DeviceGroupGuids = new Dictionary<DeviceIoType, Dictionary<DeviceType, Guid>>()
            {
                {
                    DeviceIoType.Input, new Dictionary<DeviceType, Guid>()
                    {
                        {DeviceType.Generic, GenericInputList},
                        {DeviceType.Joystick, JoystickInputList},
                        {DeviceType.Keyboard, KeyboardInputList},
                        {DeviceType.Mouse, MiceInputList}
                    }
                },
                {
                    DeviceIoType.Output, new Dictionary<DeviceType, Guid>()
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

        public static Profile CreateProfile(Context context, string title, Profile parent = null)
        {
            if (IsGlobalProfileTitle(title)) title += " not allowed";
            var profile = new Profile(context, parent)
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
                device.SubscribeDeviceBindingInput(context, deviceBinding);
            }
        }

        internal void PostLoad(Context context, Profile parentProfile = null)
        {
            this.context = context;
            ParentProfile = parentProfile;

            foreach (var profile in ChildProfiles)
            {
                profile.PostLoad(context, this);
            }

            foreach (var plugin in Plugins)
            {
                var group = plugin as PluginGroup;
                if (group != null)
                {
                    group.PostLoad(context, this);
                }
                else
                {
                    plugin.PostLoad(context, this);
                }
            }
        }
    }
}