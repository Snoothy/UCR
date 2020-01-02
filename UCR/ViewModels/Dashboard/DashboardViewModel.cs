using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Title => "Universal Control Remapper";
        public Visibility ProfileDetailsActive => SelectedProfileItem != null ? Visibility.Visible : Visibility.Hidden;
        public bool CanActivateProfile => SelectedProfileItem != null;
        public bool CanDeactivateProfile => Context?.ActiveProfile != null;
        public ProfileDeviceListControlViewModel InputDeviceControlViewModel { get; set; }
        public ProfileDeviceListControlViewModel OutputDeviceControlViewModel { get; set; }

        private ProfileItem _selectedProfileItem = null;
        public ProfileItem SelectedProfileItem
        {
            get => _selectedProfileItem;
            set
            {
                _selectedProfileItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProfileDetailsActive));
                OnPropertyChanged(nameof(CanActivateProfile));
            }
        }

        public ObservableCollection<ProfileItem> ProfileList { get; set; }
        public string ActiveProfileBreadCrumbs => Context?.ActiveProfile != null ? Context.ActiveProfile.ProfileBreadCrumbs() : "None";

        private Context Context { get; set; }

        public DashboardViewModel(Context context)
        {
            Context = context;
            ProfileList = ProfileItem.GetProfileTree(context.Profiles);
            PropertyChanged += OnPropertyChanged;
            context.ActiveProfileChangedEvent += OnActiveProfileChangedEvent;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (nameof(SelectedProfileItem).Equals(e.PropertyName) && SelectedProfileItem != null)
            {
                BuildDeviceLists();
            }
        }

        private void BuildDeviceLists()
        {
            InputDeviceControlViewModel = new ProfileDeviceListControlViewModel(SelectedProfileItem.Profile, GetDeviceConfigurations(SelectedProfileItem.Profile, DeviceIoType.Input), DeviceIoType.Input);
            OutputDeviceControlViewModel = new ProfileDeviceListControlViewModel(SelectedProfileItem.Profile, GetDeviceConfigurations(SelectedProfileItem.Profile, DeviceIoType.Output), DeviceIoType.Output);

            OnPropertyChanged(nameof(InputDeviceControlViewModel));
            OnPropertyChanged(nameof(OutputDeviceControlViewModel));
        }

        private List<DeviceConfiguration> GetDeviceConfigurations(Profile profile, DeviceIoType deviceIoType)
        {
            return SelectedProfileItem.Profile.GetDeviceConfigurationList(deviceIoType);
        }

        private void OnActiveProfileChangedEvent(Profile profile)
        {
            OnPropertyChanged(nameof(ActiveProfileBreadCrumbs));
            OnPropertyChanged(nameof(CanDeactivateProfile));
        }
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
