using System.Collections.Generic;
using System.Collections.ObjectModel;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginToolboxViewModel
    {
        public ObservableCollection<PluginItemViewModel> PluginList { get; set; }

        public PluginToolboxViewModel(List<Plugin> pluginList)
        {
            PluginList = new ObservableCollection<PluginItemViewModel>();
            foreach (var plugin in pluginList)
            {
                PluginList.Add(new PluginItemViewModel(plugin));
            }

            if (pluginList.Count > 0) PluginList[0].FirstElement = true;
        }
    }
}