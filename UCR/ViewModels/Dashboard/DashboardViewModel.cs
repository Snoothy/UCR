using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Title => "Universal Control Remapper";
        public Visibility ProfileDetailsActive => SelectedProfileItem != null ? Visibility.Visible : Visibility.Hidden;
        public bool CanActivateProfile => SelectedProfileItem != null;
        public bool CanDeactivateProfile => Context?.ActiveProfile != null;

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

            context.ActiveProfileChangedEvent += OnActiveProfileChangedEvent;
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
