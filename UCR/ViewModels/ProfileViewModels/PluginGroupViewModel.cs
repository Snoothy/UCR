using System.Collections.Generic;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginGroupViewModel
    {
        public string Group { get; set; }
        public List<PluginItemViewModel> Plugins { get; set; }

        public PluginGroupViewModel(string group)
        {
            Group = group;
            Plugins = new List<PluginItemViewModel>();
        }
    }
}