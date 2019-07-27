using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class MappingViewModel : INotifyPropertyChanged
    {
        public string MappingTitle => Mapping.FullTitle;
        public ProfileViewModel ProfileViewModel { get; }
        public Mapping Mapping { get; set; }
        public ObservableCollection<PluginViewModel> Plugins { get; set; }
        public ObservableCollection<DeviceBindingViewModel> DeviceBindings { get; set; }
        public bool ButtonsEnabled => !ProfileViewModel.Profile.IsActive();

        public MappingViewModel(ProfileViewModel profileViewModel, Mapping mapping)
        {
            ProfileViewModel = profileViewModel;
            Mapping = mapping;
            profileViewModel.Profile.Context.ActiveProfileChangedEvent += ContextOnActiveProfileChangedEvent;
            DeviceBindings = new ObservableCollection<DeviceBindingViewModel>();
            PopulateDeviceBindingsViewModels();
            PopulatePlugins(mapping);
        }

        private void ContextOnActiveProfileChangedEvent(Profile profile)
        {
            OnPropertyChanged(nameof(ButtonsEnabled));
        }

        public void Remove()
        {
            ProfileViewModel.RemoveMapping(this);
        }

        public void AddPlugin(Plugin plugin)
        {
            var newPlugin = ProfileViewModel.Profile.Context.PluginManager.GetNewPlugin(plugin);
            if (!Mapping.AddPlugin(newPlugin)) return;

            Plugins.Add(new PluginViewModel(this, newPlugin));
            if (Plugins.Count != 1) return;
            
            PopulateDeviceBindingsViewModels();
        }

        private void PopulateDeviceBindingsViewModels()
        {
            if (Mapping.Plugins.Count == 0) return;

            var plugin = Mapping.Plugins[0];
            for (var i = 0; i < plugin.InputCategories.Count; i++)
            {
                DeviceBindings.Add(new DeviceBindingViewModel(Mapping.DeviceBindings[i])
                {
                    DeviceBindingName = plugin.InputCategories[i].Name,
                    DeviceBindingCategory = plugin.InputCategories[i].Category
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

        public async void Rename()
        {
            var dialog = new StringDialog("Rename mapping", "Mapping name", Mapping.Title);
            var result = (bool?)await DialogHost.Show(dialog, ProfileViewModel.ProfileDialogIdentifier);
            if (result == null || !result.Value) return;

            Mapping.Rename(dialog.Value);
            OnPropertyChanged(nameof(MappingTitle));
        }

        public async void AddPlugin()
        {
            var dialog = new AddMappingPluginDialog(this);
            var result = (AddMappingPluginDialogViewModel)await DialogHost.Show(dialog, ProfileViewModel.ProfileDialogIdentifier);
            if (result?.SelectedPlugin == null) return;

            AddPlugin(result.SelectedPlugin.Plugin);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
