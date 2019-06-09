using System.Collections.Generic;
using System.Windows;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginPropertyGroupViewModel
    {
        public string GroupName => _pluginPropertyGroup.Title;
        public List<PluginPropertyViewModel> PluginProperties { get; set; }
        public Visibility ShowPropertyList => PluginProperties != null && PluginProperties.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

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