using System.Collections.ObjectModel;
using System.Windows;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class ProfileViewModel
    {
        public Profile Profile { get; }
        public ObservableCollection<MappingViewModel> MappingsList { get; set; }
        public ObservableCollection<ComboBoxItemViewModel> StatesList { get; set; }
        public MappingViewModel SelectedMapping { get; set; }

        public ProfileViewModel()
        {

        }

        public ProfileViewModel(Profile profile)
        {
            Profile = profile;
            PopulateMappingsList(profile);
            PopulateStatesComboBox();
        }

        private void PopulateStatesComboBox()
        {
            var states = Profile.AllStates;
            StatesList = new ObservableCollection<ComboBoxItemViewModel>();
            foreach (var state in states)
            {
                StatesList.Add(new ComboBoxItemViewModel(state.Title, state));
            }

        }

        private void PopulateMappingsList(Profile profile)
        {
            MappingsList = new ObservableCollection<MappingViewModel>();
            foreach (var profileMapping in profile.Mappings)
            {
                AddMapping(profileMapping);
            }

            if (MappingsList.Count > 0) SelectedMapping = MappingsList[0];
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

        public void RemoveMapping(MappingViewModel mappingViewModel)
        {
            if (mappingViewModel.Mapping.DeviceBindings.Count > 0)
            {
                var result =
                    MessageBox.Show("Are you sure you want to remove '" + mappingViewModel.Mapping.Title + "'?",
                        "Remove mapping?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;
            }

            if (Profile.RemoveMapping(mappingViewModel.Mapping)) MappingsList.Remove(mappingViewModel);
        }
    }
}
