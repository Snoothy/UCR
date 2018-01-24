using System;
using System.Collections.Generic;
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

        /* Persistence */
        public string Title { get; set; }
        public Guid Guid { get; set; }
        public List<Profile> ChildProfiles { get; set; }
        public List<Models.Plugin.Plugin> Plugins { get; set; }

        // IO
        public Guid InputDeviceGroupGuid { get; set; }
        public Guid OutputDeviceGroupGuid { get; set; }

        
        /* Runtime */
        [XmlIgnore]
        public Context context;
        [XmlIgnore]
        public Profile ParentProfile { get; set; }

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

            Guid = Guid == Guid.Empty ? Guid.NewGuid() : Guid;
        }

        public Profile(Context context, Profile parentProfile = null) : this(context)
        {
            ParentProfile = parentProfile;
        }

        #endregion

        #region Actions

        public static Profile CreateProfile(Context context, string title, Profile parent = null)
        {
            var profile = new Profile(context, parent)
            {
                Title = title
            };

            return profile;
        }

        public void AddNewChildProfile(string title)
        {
            if (ChildProfiles == null) ChildProfiles = new List<Profile>();
            ChildProfiles.Add(CreateProfile(context, title, this));
            context.ContextChanged();
        }

        public void AddChildProfile(Profile profile)
        {
            if (ChildProfiles == null) ChildProfiles = new List<Profile>();
            profile.context = context;
            profile.ParentProfile = this;
            ChildProfiles.Add(profile);
            context.ContextChanged();
        }

        public bool Rename(string title)
        {
            Title = title;
            context.ContextChanged();
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

        public void SetDeviceGroup(DeviceIoType ioType, Guid deviceGroupGuid)
        {
            switch (ioType)
            {
                case DeviceIoType.Input:
                    InputDeviceGroupGuid = deviceGroupGuid;
                    break;
                case DeviceIoType.Output:
                    OutputDeviceGroupGuid = deviceGroupGuid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ioType), ioType, null);
            }
            context.ContextChanged();
        }

        public bool ActivateProfile()
        {
            return context.SubscriptionsManager.ActivateProfile(this);
        }

        public bool Deactivate()
        {
            return context.SubscriptionsManager.DeactivateProfile();
        }

        #endregion

        #region Device

        public Device.Device GetDevice(DeviceBinding deviceBinding)
        {
            var deviceList = GetDeviceList(deviceBinding);
            return deviceBinding.DeviceNumber < deviceList.Count
                ? deviceList[deviceBinding.DeviceNumber]
                : null;
        }

        public List<Device.Device> GetDeviceList(DeviceBinding deviceBinding)
        {
            return GetDeviceList(deviceBinding.DeviceIoType);
        }

        private List<Device.Device> GetDeviceList(DeviceIoType deviceIoType)
        {
            return GetDeviceGroup(deviceIoType)?.Devices ?? new List<Device.Device>();
        }

        public DeviceGroup GetDeviceGroup(DeviceIoType deviceIoType)
        {
            var deviceGroupGuid = GetDeviceGroupGuid(deviceIoType);
            if (deviceGroupGuid.Equals(Guid.Empty))
            {
                return ParentProfile?.GetDeviceGroup(deviceIoType);
            }
            return context.DeviceGroupsManager.GetDeviceGroup(deviceIoType, deviceGroupGuid);
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
            return context.SubscriptionsManager.GetActiveProfile() != null && context.SubscriptionsManager.GetActiveProfile().Guid == Guid;
        }

        private Guid GetDeviceGroupGuid(DeviceIoType deviceIoType)
        {
            switch (deviceIoType)
            {
                case DeviceIoType.Input:
                    return InputDeviceGroupGuid;
                case DeviceIoType.Output:
                    return OutputDeviceGroupGuid;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deviceIoType), deviceIoType, null);
            }
        }

        public List<Profile> GetAncestry()
        {
            var result = new List<Profile>();
            if (ParentProfile != null) result.AddRange(ParentProfile.GetAncestry());
            result.Add(this);
            return result;
        }

        #endregion

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