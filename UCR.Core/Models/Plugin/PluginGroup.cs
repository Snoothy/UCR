using System;
using System.Collections.Generic;
using System.Linq;
using UCR.Core.Models.Binding;

namespace UCR.Core.Models.Plugin
{
    public abstract class PluginGroup : Plugin
    {
        public List<Plugin> Plugins { get; set; }

        public PluginGroup()
        {
            Plugins = new List<Plugin>();
        }

        public override List<DeviceBinding> GetInputs()
        {
            List<DeviceBinding> newBindings = Inputs.Select(input => new DeviceBinding(input)).ToList();

            foreach (var plugin in Plugins)
            {
                newBindings.AddRange(plugin.GetInputs());
            }

            return newBindings;
        }

        public void AddPlugin(Plugin plugin, string title)
        {
            var newPlugin = (Plugin) Activator.CreateInstance(plugin.GetType());
            newPlugin.ParentProfile = ParentProfile;
            newPlugin.Title = title;
            newPlugin.ContainingList = Plugins;
            Plugins.Add(newPlugin);
            ParentProfile.context.ContextChanged();
        }

        internal new void PostLoad(Context context, Profile.Profile parentProfile)
        {
            (this as Plugin).PostLoad(context, parentProfile);
            foreach (var plugin in Plugins)
            {
                plugin.PostLoad(context, parentProfile);
                plugin.ContainingList = Plugins;
            }
        }
    }
}
