using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Models.Subscription;

namespace HidWizards.UCR.Core.Models
{
    public class Mapping
    {
        /* Persistence */
        [XmlAttribute]
        public string Title { get; set; }
        public List<DeviceBinding> DeviceBindings { get; set; }
        public List<Plugin> Plugins { get; set; }

        /* Runtime */
        private Profile Profile { get; set; }
        private List<short> InputCache { get; set; }
        private List<CallbackMultiplexer> Multiplexer { get; set; }
        

        internal bool IsShadowMapping { get; set; }
        internal int ShadowDeviceNumber { get; set; }
        internal int PossibleShadowClones => CountPossibleShadowClones();
        internal FilterState FilterState { get; set; }

        private int CountPossibleShadowClones()
        {
            var usedDeviceConfigurations = new List<DeviceConfiguration>();

            foreach (var deviceBinding in DeviceBindings)
            {
                if (!deviceBinding.IsBound) continue;

                var deviceConfiguration = Profile.GetDeviceConfiguration(DeviceIoType.Input, deviceBinding.DeviceConfigurationGuid);
                if (deviceConfiguration != null) usedDeviceConfigurations.Add(deviceConfiguration);
            }

            if (usedDeviceConfigurations.Count == 0) return 0;

            return usedDeviceConfigurations
                .Select(deviceConfiguration => deviceConfiguration.ShadowDevices)
                .Max(shadowDevices => shadowDevices.Count);
        }

        [XmlIgnore]
        public string FullTitle
        {
            get
            {
                var mapping = GetOverridenMapping();
                return mapping != null ? $"{Title} (Overrides {GetOverridenMapping().Profile.Title})" : Title;
            }
        }

        public Mapping()
        {
            DeviceBindings = new List<DeviceBinding>();
            Plugins = new List<Plugin>();
            
            IsShadowMapping = false;
            ShadowDeviceNumber = 0;
        }

        public Mapping(Profile profile, string title) : this()
        {
            Profile = profile;
            Title = title;
        }

        public void Rename(string title)
        {
            Title = title;
            Profile.Context.ContextChanged();
        }

        internal bool IsBound()
        {
            if (DeviceBindings.Count == 0) return false;
            var result = true;
            foreach (var deviceBinding in DeviceBindings)
            {
                result &= deviceBinding.IsBound;
            }
            return result;
        }

        internal void PrepareMapping(FilterState filterState)
        {
            InputCache = new List<short>();
            DeviceBindings.ForEach(_ => InputCache.Add(0));
            Multiplexer = new List<CallbackMultiplexer>();
            for (var i = 0; i < DeviceBindings.Count; i++)
            {
                var cm = new CallbackMultiplexer(InputCache, i, Update);
                Multiplexer.Add(cm);
                DeviceBindings[i].Callback = cm.Update;
                DeviceBindings[i].CurrentValue = 0;
            }

            FilterState = filterState;
            Plugins.ForEach(p => p.RuntimeMapping = this);
        }

        internal Mapping GetOverridenMapping()
        {
            var list = new List<Mapping>();
            var parentProfile = Profile.ParentProfile;
            if (parentProfile != null) list.AddRange(parentProfile.Mappings);

            while (list.Count > 0)
            {
                var mapping = list[0];
                list.RemoveAt(0);
                if (string.Compare(Title, mapping.Title, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    return mapping;
                }

                parentProfile = parentProfile?.ParentProfile;
                if (parentProfile != null) list.AddRange(parentProfile.Mappings);
            }

            return null;
        }
        
        public void Update(short value)
        {
            foreach (var plugin in Plugins)
            {
                if (plugin.IsFiltered()) continue;
                
                plugin.Update(InputCache.ToArray());
            }
        }

        #region Plugin

        internal List<Plugin> GetPluginList()
        {
            var plugins = Profile.Context.GetPlugins();
            plugins.Sort();
            if (Plugins.Count > 0)
            {
                plugins = plugins.FindAll(p => p.HasSameInputCategories(Plugins[0]));
            }
            return plugins;
        }

        public bool AddPlugin(Plugin plugin)
        {
            if (Plugins.Count == 0)
            {
                foreach (var _ in plugin.InputCategories)
                {
                    DeviceBindings.Add(new DeviceBinding(Update, Profile, DeviceIoType.Input));
                }
            }

            plugin.SetProfile(Profile);
            Plugins.Add(plugin);

            Profile.Context.ContextChanged();
            return true;
        }

        public bool RemovePlugin(Plugin plugin)
        {
            if (!Plugins.Remove(plugin)) return false;

            if (Plugins.Count == 0)
            {
                DeviceBindings = new List<DeviceBinding>();
            }
            Profile.Context.ContextChanged();

            return true;
        }

        #endregion


        internal Mapping CreateShadowClone(int shadowCloneNumber)
        {
            var clonedMapping = Context.DeepXmlClone<Mapping>(this);
            clonedMapping.Title = $"{clonedMapping.Title} (Shadow {shadowCloneNumber})";
            clonedMapping.IsShadowMapping = true;
            clonedMapping.ShadowDeviceNumber = shadowCloneNumber;
            clonedMapping.Profile = Profile;
            clonedMapping.PostLoad(Profile.Context, Profile);

            foreach (var plugin in clonedMapping.Plugins)
            {
                plugin.Filters.ForEach(f => f.Name = Filter.GetShadowName(f.Name, shadowCloneNumber));
            }

            return clonedMapping;
        }

        internal void PostLoad(Context context, Profile profile = null)
        {
            Profile = profile;
            foreach (var deviceBinding in DeviceBindings)
            {
                deviceBinding.Profile = profile;
            }

            foreach (var plugin in Plugins)
            {
                plugin.PostLoad(context, profile);
            }
        }
    }
}
