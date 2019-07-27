using System.Collections.Generic;
using System.Collections.ObjectModel;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginToolboxViewModel
    {
        public Dictionary<string, PluginGroupViewModel> PluginGroupList { get; set; }

        public PluginToolboxViewModel(Profile profile, List<Plugin> pluginList)
        {
            PluginGroupList = new Dictionary<string, PluginGroupViewModel>();
            foreach (var plugin in pluginList)
            {
                var groupName = plugin.Group ?? "Ungrouped";
                if (!PluginGroupList.ContainsKey(groupName)) PluginGroupList.Add(groupName, new PluginGroupViewModel(groupName));
                if (!PluginGroupList.TryGetValue(groupName, out var group)) continue;
                group.Plugins.Add(new PluginItemViewModel(profile, plugin));
            }

            foreach (var pluginGroup in PluginGroupList.Values)
            {
                if (pluginGroup.Plugins.Count > 0) pluginGroup.Plugins[0].FirstElement = true;
            }
            
        }
    }
}