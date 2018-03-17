using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using HidWizards.UCR.Core.Models.Binding;
using NLog;

namespace HidWizards.UCR.Core.Models
{
    public class Profile
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /* Persistence */
        public string Title { get; set; }
        public Guid Guid { get; set; }
        public List<Profile> ChildProfiles { get; set; }
        public List<string> States { get; set; }
        public List<Mapping> Mappings { get; set; }

        // IO
        public Guid InputDeviceGroupGuid { get; set; }
        public Guid OutputDeviceGroupGuid { get; set; }

        
        /* Runtime */
        [XmlIgnore]
        public Context Context;
        [XmlIgnore]
        public Profile ParentProfile { get; set; }

        #region Constructors

        public Profile()
        {
            Init();
        }

        public Profile(Context context)
        {
            this.Context = context;
            Init();
        }

        private void Init()
        {
            Guid = Guid.NewGuid();
            ChildProfiles = new List<Profile>();
            States = new List<string>();
            Mappings = new List<Mapping>();
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
            ChildProfiles.Add(CreateProfile(Context, title, this));
            Context.ContextChanged();
        }

        public void AddChildProfile(Profile profile)
        {
            if (ChildProfiles == null) ChildProfiles = new List<Profile>();
            profile.Context = Context;
            profile.ParentProfile = this;
            ChildProfiles.Add(profile);
            Context.ContextChanged();
        }

        public bool Rename(string title)
        {
            Title = title;
            Context.ContextChanged();
            return true;
        }

        public void Remove()
        {
            if (ParentProfile == null)
            {
                Context.Profiles.Remove(this);
            }
            else
            {
                ParentProfile.ChildProfiles.Remove(this);
            }
            Context.ContextChanged();
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
            Context.ContextChanged();
        }

        public bool ActivateProfile()
        {
            return Context.SubscriptionsManager.ActivateProfile(this);
        }

        public bool Deactivate()
        {
            return Context.SubscriptionsManager.DeactivateProfile();
        }

        #endregion

        #region Mapping

        public Mapping AddMapping(string title)
        {
            var mapping = new Mapping(this, title);
            Mappings.Add(mapping);
            Context.ContextChanged();
            return mapping;
        }

        public bool RemoveMapping(Mapping mapping)
        {
            if (!Mappings.Remove(mapping)) return false;
            Context.ContextChanged();
            return true;
        }

        #endregion

        #region Device

        public Device GetDevice(DeviceBinding deviceBinding)
        {
            var deviceList = GetDeviceList(deviceBinding);
            return deviceBinding.DeviceNumber < deviceList.Count
                ? deviceList[deviceBinding.DeviceNumber]
                : null;
        }

        public List<Device> GetDeviceList(DeviceBinding deviceBinding)
        {
            return GetDeviceList(deviceBinding.DeviceIoType);
        }

        public List<Device> GetDeviceList(DeviceIoType deviceIoType)
        {
            return GetDeviceGroup(deviceIoType)?.Devices ?? new List<Device>();
        }

        public DeviceGroup GetDeviceGroup(DeviceIoType deviceIoType)
        {
            var deviceGroupGuid = GetDeviceGroupGuid(deviceIoType);
            if (deviceGroupGuid.Equals(Guid.Empty))
            {
                return ParentProfile?.GetDeviceGroup(deviceIoType);
            }
            return Context.DeviceGroupsManager.GetDeviceGroup(deviceIoType, deviceGroupGuid);
        }

        #endregion

        #region Plugin

        public bool AddNewPlugin(Mapping mapping, Plugin plugin, string state = null)
        {
            return AddPlugin(mapping, (Plugin)Activator.CreateInstance(plugin.GetType()), state);
        }

        public bool AddPlugin(Mapping mapping, Plugin plugin, string state = null)
        {
            if (!Mappings.Contains(mapping)) return false;
            plugin.State = state;
            plugin.Profile = this;
            mapping.Plugins.Add(plugin);
            Context.ContextChanged();
            return true;
        }

        public bool RemovePlugin(Mapping mapping, Plugin plugin)
        {
            if (!Mappings.Contains(mapping)) return false;
            mapping.Plugins.Remove(plugin);
            Context.ContextChanged();
            return true;
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
            return Context.SubscriptionsManager.GetActiveProfile() != null && Context.SubscriptionsManager.GetActiveProfile().Guid == Guid;
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
            Context = context;
            ParentProfile = parentProfile;

            foreach (var profile in ChildProfiles)
            {
                profile.PostLoad(context, this);
            }

            foreach (var mapping in Mappings)
            {
                mapping.PostLoad(context, this);
            }
        }
    }
}