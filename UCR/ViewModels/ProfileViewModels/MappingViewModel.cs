using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class MappingViewModel
    {
        public ProfileViewModel ProfileViewModel { get; }
        public Mapping Mapping { get; set; }
        public ObservableCollection<PluginViewModel> Plugins { get; set; }
        public ObservableCollection<DeviceBindingViewModel> DeviceBindings { get; set; }

        public MappingViewModel(ProfileViewModel profileViewModel, Mapping mapping)
        {
            ProfileViewModel = profileViewModel;
            Mapping = mapping;
            DeviceBindings = new ObservableCollection<DeviceBindingViewModel>();
            PopulateDeviceBindingsViewModels();
            PopulatePlugins(mapping);
        }

        public void Remove()
        {
            ProfileViewModel.RemoveMapping(this);
        }

        public void AddPlugin(Plugin plugin)
        {
            if (!Mapping.AddPlugin(plugin)) return;

            Plugins.Add(new PluginViewModel(this, plugin));
            if (Plugins.Count != 1) return;
            
            PopulateDeviceBindingsViewModels();
        }

        private void PopulateDeviceBindingsViewModels()
        {
            if (Mapping.Plugins.Count == 0) return;

            var plugin = Mapping.Plugins[0];
            for (var i = 0; i < plugin.GetInputCategories().Count; i++)
            {
                DeviceBindings.Add(new DeviceBindingViewModel()
                {
                    DeviceBinding = Mapping.DeviceBindings[i],
                    DeviceBindingName = plugin.GetInputCategories()[i].Name,
                    DeviceBindingCategory = plugin.GetInputCategories()[i].Category
                });
            }
        }

        public void RemovePlugin(PluginViewModel pluginViewModel)
        {
            if (!Mapping.RemovePlugin(pluginViewModel.Plugin)) return;

            Plugins.Remove(pluginViewModel);
            if (Plugins.Count == 0) DeviceBindings.Clear();
        }

        private void PopulatePlugins(Mapping mapping)
        {
            Plugins = new ObservableCollection<PluginViewModel>();
            foreach (var mappingPlugin in mapping.Plugins)
            {
                Plugins.Add(new PluginViewModel(this, mappingPlugin));
            }
        }
    }
}
