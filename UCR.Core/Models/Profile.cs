using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models.Binding;
using NLog;

namespace HidWizards.UCR.Core.Models
{
    public class Profile : INotifyPropertyChanged
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /* Persistence */
        [XmlAttribute]
        public string Title { get; set; }
        [XmlAttribute]
        public Guid Guid { get; set; }
        public List<Profile> ChildProfiles { get; set; }
        public List<Mapping> Mappings { get; set; }

        public List<DeviceConfiguration> InputDeviceConfigurations { get; set; }
        public List<DeviceConfiguration> OutputDeviceConfigurations { get; set; }


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
            Context = context;
            Init();
        }

        private void Init()
        {
            Guid = Guid.NewGuid();
            ChildProfiles = new List<Profile>();
            Mappings = new List<Mapping>();
            InputDeviceConfigurations = new List<DeviceConfiguration>();
            OutputDeviceConfigurations = new List<DeviceConfiguration>();
        }

        public Profile(Context context, Profile parentProfile = null) : this(context)
        {
            ParentProfile = parentProfile;
        }

        #endregion

        #region Actions

        public static Profile CreateProfile(Context context, string title, List<DeviceConfiguration> inputDevices,
            List<DeviceConfiguration> outputDevices, Profile parent = null)
        {
            var profile = new Profile(context, parent)
            {
                Title = title,
                InputDeviceConfigurations = inputDevices ?? new List<DeviceConfiguration>(),
                OutputDeviceConfigurations = outputDevices ?? new List<DeviceConfiguration>()
            };

            return profile;
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

        public bool ActivateProfile()
        {
            return Context.SubscriptionsManager.ActivateProfile(this);
        }

        public bool Deactivate()
        {
            return Context.SubscriptionsManager.DeactivateCurrentProfile();
        }

        internal void PrepareProfile()
        {
            
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

        public DeviceConfiguration GetDeviceConfiguration(DeviceIoType deviceIoType, Guid deviceConfigurationGuid)
        {
            var deviceList = GetDeviceConfigurationList(deviceIoType);
            return deviceList.FirstOrDefault(configuration => configuration.Guid == deviceConfigurationGuid);
        }

        public List<DeviceConfiguration> GetDeviceConfigurationList(DeviceIoType deviceIoType)
        {
            var result = new List<DeviceConfiguration>();
            if (ParentProfile != null) result.AddRange(ParentProfile.GetDeviceConfigurationList(deviceIoType));

            var devices = deviceIoType == DeviceIoType.Input ? InputDeviceConfigurations : OutputDeviceConfigurations;
            devices.ForEach(d => d.Device.Profile = this);
            result.AddRange(devices);

            return result;
        }

        public List<Device> GetMissingDeviceList(DeviceIoType deviceIoType)
        {
            Context.DevicesManager.RefreshDeviceList();
            var availableDeviceList = Context.DevicesManager.GetAvailableDeviceList(deviceIoType);
            var profileDeviceList = GetDeviceConfigurationList(deviceIoType);

            foreach (var deviceConfiguration in profileDeviceList)
            {
                availableDeviceList.RemoveAll(d => d.Equals(deviceConfiguration.Device));
            }

            return availableDeviceList;
        }

        public void AddDeviceConfigurations(List<DeviceConfiguration> deviceConfigurations, DeviceIoType deviceIoType)
        {
            deviceConfigurations.ForEach(configuration => configuration.Device.Profile = this);
            var deviceList = deviceIoType == DeviceIoType.Input ? InputDeviceConfigurations : OutputDeviceConfigurations;

            deviceList.AddRange(deviceConfigurations);
            OnPropertyChanged(deviceIoType == DeviceIoType.Input ? nameof(InputDeviceConfigurations) : nameof(OutputDeviceConfigurations));
            Context.ContextChanged();
        }

        public bool RemoveDeviceConfiguration(DeviceConfiguration device)
        {
            var success = InputDeviceConfigurations.Remove(device) || OutputDeviceConfigurations.Remove(device);
            if (success)
            {
                OnPropertyChanged(nameof(InputDeviceConfigurations));
                OnPropertyChanged(nameof(OutputDeviceConfigurations));
                Context.ContextChanged();
            }

            return success;
        }

        public bool CanRemoveDeviceConfiguration(DeviceConfiguration device)
        {
            return InputDeviceConfigurations.Contains(device) || OutputDeviceConfigurations.Contains(device);

        }
        #endregion

        #region Plugin

        public bool AddNewPlugin(Mapping mapping, Plugin plugin)
        {
            return AddPlugin(mapping, (Plugin)Activator.CreateInstance(plugin.GetType()));
        }

        public bool AddPlugin(Mapping mapping, Plugin plugin)
        {
            if (!Mappings.Contains(mapping)) return false;
            mapping.AddPlugin(plugin);
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

        public HashSet<string> GetFilters()
        {
            var result = ParentProfile != null
                ? ParentProfile.GetFilters()
                : new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var mapping in Mappings)
            {
                foreach (var plugin in mapping.Plugins)
                {
                    foreach (var filter in plugin.Filters)
                    {
                        result.Add(filter.Name);
                    }
                }
            }

            return result;
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}