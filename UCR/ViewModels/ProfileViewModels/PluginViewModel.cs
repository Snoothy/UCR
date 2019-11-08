using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginViewModel : INotifyPropertyChanged
    {
        public MappingViewModel MappingViewModel { get; }
        public Plugin Plugin { get; set; }
        public ObservableCollection<DeviceBindingViewModel> DeviceBindings { get; set; }
        public ObservableCollection<PluginPropertyGroupViewModel> PluginPropertyGroups { get; set; }
        public bool CanRemove => !MappingViewModel.ProfileViewModel.Profile.IsActive() && MappingViewModel.Plugins.Count > 1;
        public bool CanAddFilter => !MappingViewModel.ProfileViewModel.Profile.IsActive();
        public ObservableCollection<FilterViewModel> Filters { get; set; }

        public PluginViewModel(MappingViewModel mappingViewModel, Plugin plugin)
        {
            MappingViewModel = mappingViewModel;
            Plugin = plugin;
            mappingViewModel.ProfileViewModel.Profile.Context.ActiveProfileChangedEvent += ContextOnActiveProfileChangedEvent;
            mappingViewModel.Plugins.CollectionChanged += Plugins_CollectionChanged;
            PopulateDeviceBindingsViewModels();
            PopulatePluginProperties();
            PopulateFilterViewModels();
        }

        private void PopulateFilterViewModels()
        {
            Filters = new ObservableCollection<FilterViewModel>();
            foreach (var filter in Plugin.Filters)
            {
                Filters.Add(new FilterViewModel(this, filter));
            }
        }

        private void Plugins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CanRemove));
        }

        private void ContextOnActiveProfileChangedEvent(Profile profile)
        {
            OnPropertyChanged(nameof(CanRemove));
            OnPropertyChanged(nameof(CanAddFilter));
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
                DeviceBindings.Add(new DeviceBindingViewModel(Plugin.Outputs[i])
                {
                    DeviceBindingName = Plugin.OutputCategories[i].Name,
                    DeviceBindingCategory = Plugin.OutputCategories[i].Category,
                    PluginPropertyGroup = GetPluginPropertyGroupForOutput(Plugin.OutputCategories[i].GroupName)
                });
            }
        }

        private PluginPropertyGroupViewModel GetPluginPropertyGroupForOutput(string groupName)
        {
            if (groupName == null) return null;
            return new PluginPropertyGroupViewModel(Plugin.GetGuiMatrix().Find(p => p.GroupName.Equals(groupName)));
        }

        private void PopulatePluginProperties()
        {
            PluginPropertyGroups = new ObservableCollection<PluginPropertyGroupViewModel>();
            foreach (var pluginPropertyGroup in Plugin.PluginPropertyGroups)
            {
                if (pluginPropertyGroup.PluginProperties.Count == 0 || pluginPropertyGroup.GroupType.Equals(PluginPropertyGroup.GroupTypes.Output)) continue;

                PluginPropertyGroups.Add(new PluginPropertyGroupViewModel(pluginPropertyGroup));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void AddFilter()
        {
            var dialog = new StringDialog("Add filter", "Filter name", "");
            var result = (bool?)await DialogHost.Show(dialog, MappingViewModel.ProfileViewModel.ProfileDialogIdentifier);
            if (result == null || !result.Value) return;

            var filterViewModel = new FilterViewModel(this, Plugin.AddFilter(dialog.Value));
            Filters.Add(filterViewModel);
        }

        public void RemoveFilter(FilterViewModel filterViewModel)
        {
            if (Plugin.RemoveFilter(filterViewModel.Filter)) Filters.Remove(filterViewModel);
        }

        public void ToggleFilter(FilterViewModel filterViewModel)
        {
            Plugin.ToggleFilter(filterViewModel.Filter);
        }
    }
}
