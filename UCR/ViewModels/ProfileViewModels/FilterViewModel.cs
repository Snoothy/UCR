using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;
using MaterialDesignThemes.Wpf;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class FilterViewModel : INotifyPropertyChanged
    {

        public string Name => Filter.Name;
        public bool Negative => Filter.Negative;
        public Filter Filter { get; }
        public PackIconKind ChipIcon => Negative ? PackIconKind.Minus : PackIconKind.Plus;
        public bool IsEnabled => !_pluginViewModel.MappingViewModel.ProfileViewModel.Profile.IsActive();

        private readonly PluginViewModel _pluginViewModel;

        public FilterViewModel(PluginViewModel pluginViewModel, Filter filter)
        {
            _pluginViewModel = pluginViewModel;
            Filter = filter;
            _pluginViewModel.MappingViewModel.ProfileViewModel.Profile.Context.ActiveProfileChangedEvent += ContextOnActiveProfileChangedEvent;
        }

        private void ContextOnActiveProfileChangedEvent(Profile profile)
        {
            OnPropertyChanged(nameof(IsEnabled));
        }

        public void ToggleFilter()
        {
            _pluginViewModel.ToggleFilter(this);
            OnPropertyChanged(nameof(ChipIcon));
        }

        public void RemoveFilter()
        {
            _pluginViewModel.RemoveFilter(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}