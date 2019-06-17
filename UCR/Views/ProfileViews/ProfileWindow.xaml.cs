using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels.ProfileViewModels;
using HidWizards.UCR.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace HidWizards.UCR.Views.ProfileViews
{
    public partial class ProfileWindow : Window
    {
        public Guid ProfileGuid => Profile.Guid;
        private Context Context { get; }
        private Profile Profile { get; }
        private ProfileViewModel ProfileViewModel { get; }
        private DispatcherTimer DispatcherTimer { get; set; }
        private List<DeviceBindingViewModel> DeviceBindingViewModels { get; set; }

        public ProfileWindow(Context context, Profile profile)
        {
            Context = context;
            Profile = profile;
            ProfileViewModel = new ProfileViewModel(profile);
            InitializeComponent();
            Title = "Edit " + profile.Title;
            DataContext = ProfileViewModel;
            context.ActiveProfileChangedEvent += ContextOnActiveProfileChangedEvent;
            StartGuiTimer();
        }

        private void Save_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Context.SaveContext();
        }

        private void Save_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Context.IsNotSaved;
        }

        #region GUI

        private void StartGuiTimer()
        {
            if (!Profile.IsActive()) return;
            DispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);
            DispatcherTimer.Interval = TimeSpan.FromMilliseconds(15);
            DispatcherTimer.Tick += DispatcherTimerOnTick;

            DeviceBindingViewModels = new List<DeviceBindingViewModel>();
            foreach (var mappingViewModel in ProfileViewModel.MappingsList)
            {
                DeviceBindingViewModels.AddRange(mappingViewModel.DeviceBindings);
                foreach (var pluginViewModel in mappingViewModel.Plugins)
                {
                    DeviceBindingViewModels.AddRange(pluginViewModel.DeviceBindings);
                }
            }

            DispatcherTimer.Start();
        }

        private void StopGuiTimer()
        {
            DispatcherTimer?.Stop();
            DispatcherTimer = null;
        }

        private void ContextOnActiveProfileChangedEvent(Profile profile)
        {
            if (profile == null)
            {
                StopGuiTimer();
                return;
            }

            if (profile.Guid != ProfileGuid) return;
            StartGuiTimer();
        }

        private void DispatcherTimerOnTick(object sender, EventArgs e)
        {
            if (!IsActive) return;
            DeviceBindingViewModels.ForEach(d => d.CurrentValueChanged());
        }

        #endregion

        #region Profile

        private void ActivateProfile(object sender, RoutedEventArgs e)
        {
            if (!Profile.ActivateProfile())
            {
                MessageBox.Show("The Profile could not be activated, see the log for more details", "Profile failed to activate!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DeactivateProfile(object sender, RoutedEventArgs e)
        {
            Profile.Deactivate();
        }

        #endregion

        private async void AddMapping_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            if (!(button.DataContext is PluginItemViewModel pluginItem)) return;

            var dialog = new StringDialog("Create mapping for: " + pluginItem.Name, "Mapping name", "");
            var result = (bool?)await DialogHost.Show(dialog, ProfileViewModel.ProfileDialogIdentifier);
            if (result == null || !result.Value) return;

            var mappingViewModel = ProfileViewModel.AddMapping(dialog.Value);
            mappingViewModel.AddPlugin(Profile.Context.PluginManager.GetNewPlugin(pluginItem.Plugin));
            MappingListView.ScrollIntoView(MappingListView.Items[MappingListView.Items.Count - 1]);

        }
    }
}
