using System.Collections.Generic;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginPropertyGroupViewModel
    {
        public string GroupName => _pluginPropertyGroup.GroupName;
        public List<PluginProperty> PluginProperties => _pluginPropertyGroup.PluginProperties;

        private readonly PluginPropertyGroup _pluginPropertyGroup;
        

        public PluginPropertyGroupViewModel(PluginPropertyGroup pluginPropertyGroup)
        {
            _pluginPropertyGroup = pluginPropertyGroup;
        }
    }
}