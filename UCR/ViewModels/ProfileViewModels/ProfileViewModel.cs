using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        public Profile Profile { get; }
        public bool CanActivateProfile => Profile.Context.ActiveProfile != Profile;
        public bool CanDeactivateProfile => Profile.Context.ActiveProfile != null;
        public ObservableCollection<MappingViewModel> MappingsList { get; set; }
        public PluginToolboxViewModel PluginToolbox { get; set; }
        public string ProfileDialogIdentifier => $"ProfileDialog-{Profile.Guid}";

        public ProfileViewModel()
        {

        }

        public ProfileViewModel(Profile profile)
        {
            Profile = profile;
            profile.Context.ActiveProfileChangedEvent += ContextOnActiveProfileChangedEvent;
            PopulateMappingsList(profile);
            var pluginList = profile.Context.GetPlugins();
            pluginList.Sort();
            PluginToolbox = new PluginToolboxViewModel(profile, pluginList);
        }

        private void ContextOnActiveProfileChangedEvent(Profile profile)
        {
            OnPropertyChanged(nameof(CanActivateProfile));
            OnPropertyChanged(nameof(CanDeactivateProfile));
        }

        private void PopulateMappingsList(Profile profile)
        {
            MappingsList = new ObservableCollection<MappingViewModel>();
            foreach (var profileMapping in profile.Mappings)
            {
                AddMapping(profileMapping);
            }
        }

        public MappingViewModel AddMapping(string title)
        {
            return AddMapping(Profile.AddMapping(title));
        }

        public MappingViewModel AddMapping(Mapping mapping)
        {
            var mappingViewModel = new MappingViewModel(this, mapping);
            MappingsList.Add(mappingViewModel);
            return mappingViewModel;
        }

        public async void RemoveMapping(MappingViewModel mappingViewModel)
        {
            if (mappingViewModel.Mapping.DeviceBindings.Count > 0)
            {
                var dialog = new BoolDialog("Remove mapping", "Are you sure you want to remove the mapping: " + mappingViewModel.Mapping.Title + "?");
                var result = (bool?)await DialogHost.Show(dialog, ProfileDialogIdentifier);
                if (result == null || !result.Value) return;
            }

            if (Profile.RemoveMapping(mappingViewModel.Mapping)) MappingsList.Remove(mappingViewModel);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
