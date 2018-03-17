using System;
using System.Collections.Generic;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Models
{
    public class Mapping
    {
        // Persistence
        public String Title { get; set; }
        public Guid Guid { get; set; }
        public List<DeviceBinding> DeviceBindings { get; set; }
        public List<Plugin> Plugins { get; set; }

        // Runtime
        internal Profile Profile { get; set; }
        private List<long> InputCache { get; set; }

        public Mapping()
        {
            Guid = Guid.NewGuid();
            DeviceBindings = new List<DeviceBinding>();
            Plugins = new List<Plugin>();
        }

        public Mapping(Profile profile, string title) : this()
        {
            Profile = profile;
            Title = title;
        }

        internal void PrepareMapping()
        {
            InputCache = new List<long>();
            foreach (var _ in DeviceBindings)
            {
                InputCache.Add(0L);
            }
        }

        // TODO Add Guid to distinguish devicebindings
        internal void Update(long value)
        {
            InputCache[0] = value;
            foreach (var plugin in Plugins)
            {
                plugin.WriteOutput(plugin.Update(InputCache));
            }
        }

        #region Plugin

        internal List<Plugin> GetPluginList()
        {
            var plugins = Profile.Context.GetPlugins();
            plugins.Sort();
            return plugins;
        }

        public bool AddPlugin(Plugin plugin)
        {
            if (Plugins.Count == 0)
            {
                foreach (var _ in plugin.GetInputCategories())
                {
                    DeviceBindings.Add(new DeviceBinding(Update, Profile, DeviceIoType.Input));
                }
            }
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

        internal void InitializeMappings(int amount)
        {
            DeviceBindings = new List<DeviceBinding>();
            for (var i = 0; i < amount; i++)
            {
                DeviceBindings.Add(new DeviceBinding(Update, Profile, DeviceIoType.Input));
            }
        }
    }
}
