using System.Collections.ObjectModel;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginViewModel
    {
        public MappingViewModel MappingViewModel { get; }
        public Plugin Plugin { get; set; }
        public ObservableCollection<DeviceBindingViewModel> DeviceBindings { get; set; }

        public PluginViewModel(MappingViewModel mappingViewModel, Plugin plugin)
        {
            MappingViewModel = mappingViewModel;
            Plugin = plugin;

            PopulateDeviceBindingsViewModels();
        }

        public void Remove()
        {
            MappingViewModel.RemovePlugin(this);
        }

        private void PopulateDeviceBindingsViewModels()
        {
            DeviceBindings = new ObservableCollection<DeviceBindingViewModel>();
            for (var i = 0; i < Plugin.OutputCategories.Count; i++)
            {
                DeviceBindings.Add(new DeviceBindingViewModel()
                {
                    DeviceBinding = Plugin.Outputs[i],
                    DeviceBindingName = Plugin.OutputCategories[i].Name,
                    DeviceBindingCategory = Plugin.OutputCategories[i].Category
                });
            }
        }
    }
}
