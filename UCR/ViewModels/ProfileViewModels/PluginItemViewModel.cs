using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginItemViewModel : INotifyPropertyChanged
    {
        public string Name => Plugin.PluginName;
        public string Description => Plugin.Description;
        public Visibility SeparatorVisibility => FirstElement ? Visibility.Collapsed : Visibility.Visible;
        public bool FirstElement { get; set; }
        public bool IsEnabled => !_profile.IsActive();

        public Plugin Plugin { get; }

        private readonly Profile _profile;

        public PluginItemViewModel(Profile profile, Plugin plugin)
        {
            Plugin = plugin;
            _profile = profile;
            profile.Context.ActiveProfileChangedEvent += ContextOnActiveProfileChangedEvent;
        }

        private void ContextOnActiveProfileChangedEvent(Profile profile)
        {
            OnPropertyChanged(nameof(IsEnabled));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}