using System.Collections.Generic;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginPropertyGroupViewModel
    {
        public string GroupName => _pluginPropertyGroup.GroupName;
        public List<PluginPropertyViewModel> PluginProperties { get; set; }
    
        private readonly PluginPropertyGroup _pluginPropertyGroup;
        

        public PluginPropertyGroupViewModel(PluginPropertyGroup pluginPropertyGroup)
        {
            _pluginPropertyGroup = pluginPropertyGroup;
            PluginProperties = GetPluginPropertyViewModel(pluginPropertyGroup.PluginProperties);
        }

        private List<PluginPropertyViewModel> GetPluginPropertyViewModel(List<PluginProperty> pluginProperties)
        {
            var pluginPropertyViewModel = new List<PluginPropertyViewModel>();

            foreach (var pluginProperty in pluginProperties)
            {
                pluginPropertyViewModel.Add(new PluginPropertyViewModel(pluginProperty));
            }

            if (pluginPropertyViewModel.Count > 0)  pluginPropertyViewModel[pluginPropertyViewModel.Count - 1].LastElement = true;
            return pluginPropertyViewModel;
        }
    }
}