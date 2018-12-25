using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Title => "Universal Control Remapper";

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
        }
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
