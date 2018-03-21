using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginViewModel
    {
        public MappingViewModel MappingViewModel { get; }
        public Plugin Plugin { get; set; }

        public PluginViewModel(MappingViewModel mappingViewModel, Plugin plugin)
        {
            MappingViewModel = mappingViewModel;
            Plugin = plugin;
        }

        public void Remove()
        {
            MappingViewModel.RemovePlugin(this);
        }
    }
}
