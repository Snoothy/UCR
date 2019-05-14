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
        public string Title { get; set; }
        public Guid Guid { get; set; }
        public List<Profile> ChildProfiles { get; set; }
        public List<State> States { get; set; }
        public List<Mapping> Mappings { get; set; }

        public List<Device> InputDevices { get; set; }
        public List<Device> OutputDevices { get; set; }

        /* Property helpers */
        [XmlIgnore]
        public List<State> AllStates
        {
            get
            {
                var list = new List<State>
                {
                    new State
                    {
                        Guid = Guid.Empty,
                        Title = "Default"
                    }
                };
                list.AddRange(States);
                return list;
            }
        }


        /* Runtime */
        [XmlIgnore]
        public Context Context;
        [XmlIgnore]
        public Profile ParentProfile { get; set; }
        internal ConcurrentDictionary<Guid, bool> StateDictionary { get; set; }
        
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
            States = new List<State>();
            Mappings = new List<Mapping>();
            InputDevices = new List<Device>();
            OutputDevices = new List<Device>();
        }

        public Profile(Context context, Profile parentProfile = null) : this(context)
        {
            ParentProfile = parentProfile;
        }

        #endregion

        #region Actions

        public static Profile CreateProfile(Context context, string title, List<Device> inputDevices,
            List<Device> outputDevices, Profile parent = null)
        {
            var profile = new Profile(context, parent)
            {
                Title = title,
                InputDevices = inputDevices,
                OutputDevices = outputDevices
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
            return Context.SubscriptionsManager.DeactivateProfile();
        }

        internal void PrepareProfile()
        {
            StateDictionary = new ConcurrentDictionary<Guid, bool>();
            States.ForEach(s => StateDictionary.TryAdd(s.Guid, false));
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

        public Device GetDevice(DeviceIoType deviceIoType, Guid deviceGuid)
        {
            var deviceList = GetDeviceList(deviceIoType);
            return deviceList.FirstOrDefault(d => d.Guid == deviceGuid);
        }

        public List<Device> GetDeviceList(DeviceIoType deviceIoType)
        {
            var result = new List<Device>();
            if (ParentProfile != null) result.AddRange(ParentProfile.GetDeviceList(deviceIoType));

            var devices = deviceIoType == DeviceIoType.Input ? InputDevices : OutputDevices;
            devices.ForEach(d => d.Profile = this);
            result.AddRange(devices);

            return result;
        }

        public List<Device> GetMissingDeviceList(DeviceIoType deviceIoType)
        {
            var availableDeviceGroupList = Context.DevicesManager.GetAvailableDeviceList(deviceIoType);
            var availableDeviceList = new List<Device>();
            var profileDeviceList = GetDeviceList(deviceIoType);

            foreach (var deviceGroup in availableDeviceGroupList)
            {
                availableDeviceList.AddRange(deviceGroup.Devices);
            }

            foreach (var device in profileDeviceList)
            {
                availableDeviceList.RemoveAll(d => d.Equals(device));
            }

            return availableDeviceList;
        }

        public void AddDevices(List<Device> devices, DeviceIoType deviceIoType)
        {
            devices.ForEach(d => d.Profile = this);
            var deviceList = deviceIoType == DeviceIoType.Input ? InputDevices : OutputDevices;
            deviceList.AddRange(devices);
            OnPropertyChanged(deviceIoType == DeviceIoType.Input ? nameof(InputDevices) : nameof(OutputDevices));
            Context.ContextChanged();
        }

        public bool RemoveDevice(Device device)
        {
            var success = InputDevices.Remove(device) || OutputDevices.Remove(device);
            if (success)
            {
                OnPropertyChanged(nameof(InputDevices));
                OnPropertyChanged(nameof(OutputDevices));
                Context.ContextChanged();
            }

            return success;
        }


        public bool CanRemoveDevice(Device device)
        {
            return InputDevices.Contains(device) || OutputDevices.Contains(device);

        }
        #endregion

        #region Plugin

        public bool AddNewPlugin(Mapping mapping, Plugin plugin, Guid? state = null)
        {
            return AddPlugin(mapping, (Plugin)Activator.CreateInstance(plugin.GetType()), state);
        }

        public bool AddPlugin(Mapping mapping, Plugin plugin, Guid? state = null)
        {
            if (!Mappings.Contains(mapping)) return false;
            plugin.State = state ?? Guid.Empty;
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

        #region State

        public State AddState(string title)
        {
            if (States.Find(s => s.Title == title) != null) return null;
            var newState = new State(title);
            States.Add(newState);
            Context.ContextChanged();
            return newState;
        }

        public bool RemoveState(State state)
        {
            var success = States.Remove(state);
            // TODO remove all plugins with state

            Context.ContextChanged();
            return success;
        }

        public string GetStateTitle(Guid stateGuid)
        {
            var state = States.Find(s => s.Guid == stateGuid);
            return state == null ? "" : state.Title;
        }

        public void SetRuntimeState(Guid stateGuid, bool newState)
        {
            if (StateDictionary.TryGetValue(stateGuid, out var currentState))
            {
                StateDictionary.TryUpdate(stateGuid, newState, currentState);
            }
        }

        public bool GetRuntimeState(Guid stateGuid)
        {
            return StateDictionary.TryGetValue(stateGuid, out var currentState) && currentState;
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