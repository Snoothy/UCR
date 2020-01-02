using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Utilities;
using HidWizards.UCR.ViewModels.Dashboard;
using HidWizards.UCR.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using ProfileWindow = HidWizards.UCR.Views.ProfileViews.ProfileWindow;

namespace HidWizards.UCR.Views
{

    public partial class MainWindow : Window
    {
        private Context Context { get; set; }
        private readonly DashboardViewModel _dashboardViewModel;
        private CloseState WindowCloseState { get; set; }
        private Dictionary<Guid, ProfileWindow> ProfileWindows;

        enum CloseState
        {
            None,
            Closing,
            ForceClose
        }

        public MainWindow(Context context)
        {
            _dashboardViewModel = new DashboardViewModel(context);
            DataContext = _dashboardViewModel;
            Context = context;
            ProfileWindows = new Dictionary<Guid, ProfileWindow>();
            InitializeComponent();
        }

        /// <summary>
        /// AddHook Handle WndProc messages in WPF
        /// This cannot be done in a Window's constructor as a handle window handle won't at that point, so there won't be a HwndSource.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            EnableMessageHandling();
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            hwndSource?.AddHook(WndProc);
        }

        private bool GetSelectedItem(out ProfileItem profileItem)
        {
            var pi = ProfileTree.SelectedItem as ProfileItem;
            if (pi == null)
            {
                MessageBox.Show("Please select a Profile", "No Profile selected!",MessageBoxButton.OK, MessageBoxImage.Exclamation);
                profileItem = null;
                return false;
            }
            profileItem = pi;
            return true;
        }

        // TODO Deprecated, replace with property notifications
        private void ReloadProfileTree()
        {
            var profileTree = ProfileItem.GetProfileTree(Context.Profiles);
            ProfileTree.ItemsSource = profileTree;
        }


        #region Profile Actions

        private void ActivateProfile(object sender, RoutedEventArgs e)
        {
            if (!GetSelectedItem(out var profileItem)) return;
            if (!Context.SubscriptionsManager.ActivateProfile(profileItem.Profile))
            {
                // TODO Move to dialog
                MessageBox.Show("The Profile could not be activated, see the log for more details", "Profile failed to activate!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DeactivateProfile(object sender, RoutedEventArgs e)
        {
            if (Context.ActiveProfile == null) return;
            
            if (!Context.SubscriptionsManager.DeactivateCurrentProfile())
            {
                // TODO Move to dialog
                MessageBox.Show("The active Profile could not be deactivated, see the log for more details", "Profile failed to deactivate!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private async void AddProfile(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateProfileDialog("Create profile", Context.DevicesManager);
            var result = (CreateProfileDialogViewModel) await DialogHost.Show(dialog, "RootDialog");
            if (result == null || string.IsNullOrEmpty(result.ProfileName)) return;

            var inputs = result.GetInputDevices().ConvertAll(d => new DeviceConfiguration(d));
            var outputs = result.GetOutputDevices().ConvertAll(d => new DeviceConfiguration(d));

            var profile = Context.ProfilesManager.CreateProfile(result.ProfileName, inputs, outputs);
            Context.ProfilesManager.AddProfile(profile);

            ReloadProfileTree();
            OpenProfileWindow(profile);
        }

        private async void AddChildProfile(object sender, RoutedEventArgs e)
        {
            if (!GetSelectedItem(out var profileItem)) return;
            var dialog = new CreateProfileDialog("Create child profile", Context.DevicesManager);
            var result = (CreateProfileDialogViewModel)await DialogHost.Show(dialog, "RootDialog");
            if (result == null || string.IsNullOrEmpty(result.ProfileName)) return;

            var inputs = result.GetInputDevices().ConvertAll(d => new DeviceConfiguration(d));
            var outputs = result.GetOutputDevices().ConvertAll(d => new DeviceConfiguration(d));

            var profile = Context.ProfilesManager.CreateProfile(result.ProfileName, inputs, outputs);
            Context.ProfilesManager.AddProfile(profile, profileItem.Profile);

            ReloadProfileTree();
            OpenProfileWindow(profile);
        }

        private void EditProfile(object sender, RoutedEventArgs e)
        {
            if (!GetSelectedItem(out var profileItem)) return;

            if (sender is TreeViewItem)
            {
                var senderItem = ((TreeViewItem)sender).DataContext as ProfileItem;
                if (!profileItem.Id.Equals(senderItem?.Id)) return;
            }

            if (ProfileWindows.TryGetValue(profileItem.Profile.Guid, out var profileWindow))
            {
                void FocusAction() => profileWindow.Focus();
                Dispatcher.BeginInvoke((Action) FocusAction);
                return;
            }
            
            OpenProfileWindow(profileItem.Profile);
        }

        private void OpenProfileWindow(Profile profile)
        {
            void ShowAction()
            {
                var win = new ProfileWindow(Context, profile);
                ProfileWindows.Add(win.ProfileGuid, win);
                win.Closed += OnProfileWindowClosed;
                win.Focus();
                win.Show();
                var restoreFocusDialogClose = RootDialog.GetType().GetField("_restoreFocusDialogClose", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                restoreFocusDialogClose?.SetValue(RootDialog, null);
            }

            Dispatcher.BeginInvoke((Action)ShowAction);
        }

        private void OnProfileWindowClosed(object sender, EventArgs e)
        {
            if (sender is ProfileWindow window)
            {
                ProfileWindows.Remove(window.ProfileGuid);
            }
        }

        private async void RenameProfile(object sender, RoutedEventArgs e)
        {
            if (!GetSelectedItem(out var profileItem)) return;
            var dialog = new StringDialog("Rename profile", "Profile name", profileItem.Profile.Title);
            var result = (bool?)await DialogHost.Show(dialog, "RootDialog");
            if (result == null || !result.Value) return;

            profileItem.Profile.Rename(dialog.Value);
            ReloadProfileTree();
        }

        private async void CopyProfile(object sender, RoutedEventArgs e)
        {
            if (!GetSelectedItem(out var profileItem)) return;
            var dialog = new StringDialog("Copy profile", "Profile name", profileItem.Profile.Title + " Copy");
            var result = (bool?)await DialogHost.Show(dialog, "RootDialog");
            if (result == null || !result.Value) return;

            Context.ProfilesManager.CopyProfile(profileItem.Profile, dialog.Value);
            ReloadProfileTree();
        }

        private async void RemoveProfile(object sender, RoutedEventArgs e)
        {
            if (!GetSelectedItem(out var profileItem)) return;
            var dialog = new BoolDialog("Remove profile","Are you sure you want to remove: " + profileItem.Profile.Title + "?");
            var result = (bool?)await DialogHost.Show(dialog, "RootDialog");
            if (result == null || !result.Value) return;

            profileItem.Profile.Remove();
            ReloadProfileTree();
        }

        #endregion Profile Actions

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (CloseState.ForceClose.Equals(WindowCloseState)) return;
            if (CloseState.Closing.Equals(WindowCloseState))
            {
                if (WindowState.Equals(WindowState.Minimized)) WindowState = WindowState.Normal;
                
                e.Cancel = true;
                SystemSounds.Exclamation.Play();
                return;
            }

            WindowCloseState = CloseState.Closing;

            if (Context.IsNotSaved)
            {
                e.Cancel = true;

                if (WindowState.Equals(WindowState.Minimized))
                {
                    WindowState = WindowState.Normal;
                    SystemSounds.Exclamation.Play();
                    RootDialog.Focus();
                }

                if (RootDialog.IsOpen)
                {
                    DialogHost.CloseDialogCommand.Execute(null, RootDialog);
                }

                var dialog = new DecisionDialog("Configuration has changed", "Do you want to save before closing?");
                var result = (MessageBoxResult?)await DialogHost.Show(dialog, "RootDialog");
                if (result == null) return;

                switch (result)
                {
                    case MessageBoxResult.None:
                    case MessageBoxResult.Cancel:
                        WindowCloseState = CloseState.None;
                        return;
                    case MessageBoxResult.OK:
                    case MessageBoxResult.Yes:
                        Context.SaveContext();
                        WindowCloseState = CloseState.ForceClose;
                        Close();
                        break;
                    case MessageBoxResult.No:
                        WindowCloseState = CloseState.ForceClose;
                        Close();
                        break;
                }
            }
        }
        
        private void Save_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Context.SaveContext();
        }

        private void Save_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Context.IsNotSaved;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != NativeMethods.WM_COPYDATA) return IntPtr.Zero;
            
            var data = (NativeMethods.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(NativeMethods.COPYDATASTRUCT));
            var argsString = Marshal.PtrToStringAnsi(data.lpData);
            if (!string.IsNullOrEmpty(argsString)) Context.ParseCommandLineArguments(argsString.Split(';'));
            return IntPtr.Zero;
        }

        private void EnableMessageHandling()
        {
            var changeFilter = new NativeMethods.CHANGEFILTERSTRUCT();
            changeFilter.size = (uint)Marshal.SizeOf(changeFilter);
            changeFilter.info = 0;
            if
            (
                NativeMethods.ChangeWindowMessageFilterEx(
                    new WindowInteropHelper(this).EnsureHandle(),
                    NativeMethods.WM_COPYDATA,
                    NativeMethods.ChangeWindowMessageFilterExAction.Allow,
                    ref changeFilter)
            ) return;

            var error = Marshal.GetLastWin32Error();
            MessageBox.Show($"Enabling message handling failed with the error: {error}");
        }

        private async void About_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new AboutDialog();
            await DialogHost.Show(dialog, "RootDialog");
        }

        private async void Help_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new HelpDialog();
            await DialogHost.Show(dialog, "RootDialog");
        }

        private void ProfileTree_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            _dashboardViewModel.SelectedProfileItem = treeView?.SelectedItem as ProfileItem;
        }
    }
}
