using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using HidWizards.UCR.Core;
using HidWizards.UCR.Properties;
using HidWizards.UCR.Utilities;
using HidWizards.UCR.ViewModels;
using HidWizards.UCR.Views.ProfileViews;
using HidWizards.UCR.Settings;
using UCR.Views.ProfileViews;
using DeviceListWindow = HidWizards.UCR.Views.DeviceViews.DeviceListWindow;
using ProfileWindow = HidWizards.UCR.Views.ProfileViews.ProfileWindow;

namespace HidWizards.UCR.Views
{
    public partial class MainWindow : Window, INotifyPropertyChanged, ISettingsProvider
    {
        private readonly bool _isLoaded;

        private readonly ISettingsProvider _settings;

        private Context context;

        public string ActiveProfileBreadCrumbs => context?.ActiveProfile != null ? context.ActiveProfile.ProfileBreadCrumbs() : "None";

        public bool StartMinimized { get => _settings.StartMinimized; set => _settings.StartMinimized = value; }
        public Point WindowLocation { get => _settings.WindowLocation; set => _settings.WindowLocation = value; }
        public Size WindowSize { get => _settings.WindowSize; set => _settings.WindowSize = value; }

        public MainWindow(Context context)
        {
            DataContext = this;
            this.context = context;
            InitializeComponent();

            context.SetActiveProfileCallback(ActiveProfileChanged);
            ReloadProfileTree();
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
        }

        #region Get/Set settings

        protected ISettingsProvider GetSettings()
        {
            return (ISettingsProvider)new ConfigSettings(this);
        }

        private class ConfigSettings : SettingsProvider
        {
            private MainWindow mainWindow;
            private const string START_MINIMIZED = nameof(StartMinimized);
            private const string WINDOW_LOCATION = nameof(WindowLocation);
            private const string WINDOW_SIZE = nameof(WindowSize);

            public ConfigSettings(MainWindow mainWindow)

                : base(UCR.Properties.Settings.Default,
                START_MINIMIZED,
                WINDOW_LOCATION,
                WINDOW_SIZE)
            {
                this.mainWindow = mainWindow;
            }
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            // We need to delay this call because we are
            // notified of a location change before a
            // window state change.  That causes a problem
            // when maximizing the window because we record
            // the maximized window's location, which is not
            // something worth saving.
            if (_isLoaded && base.WindowState == WindowState.Normal)
            {
                var wLocation = new Point(base.Left, base.Top);
                _settings.WindowLocation = wLocation;
            };
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo info)
        {
            base.OnRenderSizeChanged(info);

            if (_isLoaded && base.WindowState == WindowState.Normal)
            {
                _settings.WindowSize = base.RenderSize;
            }
        }

        private void ApplySettings()
        {
            // WindowState
            var wState = _settings.StartMinimized;
            this.WindowState = wState ? WindowState.Minimized : WindowState.Normal;

            // WindowSize
            var wSize = _settings.WindowSize;
            this.Width = wSize.Width;
            this.Height = wSize.Height;

            // WindowLocation
            var wLocation = _settings.WindowLocation;

            // If the user's machine had two monitors but now only
            // has one, and the Window was previously on the other
            // monitor, we need to move the Window into view.
            var outOfBounds =
                wLocation.X <= -wSize.Width ||
                wLocation.Y <= -wSize.Height ||
                SystemParameters.VirtualScreenWidth <= wLocation.X ||
                SystemParameters.VirtualScreenHeight <= wLocation.Y;
        }

        #endregion Get/Set settings

        private bool GetSelectedItem(out ProfileItem profileItem)
        {
            var pi = ProfileTree.SelectedItem as ProfileItem;
            if (pi == null)
            {
                MessageBox.Show("Please select a profile", "No profile selected!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                profileItem = null;
                return false;
            }
            profileItem = pi;
            return true;
        }

        private void ReloadProfileTree()
        {
            var profileTree = ProfileItem.GetProfileTree(context.Profiles);
            ProfileTree.ItemsSource = profileTree;
        }

        #region Profile Actions

        private void ActivateProfile(object sender, RoutedEventArgs e)
        {
            var a = sender as ProfileItem;
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            if (!context.SubscriptionsManager.ActivateProfile(pi.profile))
            {
                MessageBox.Show("The profile could not be activated, see the log for more details", "Profile failed to activate!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DeactivateProfile(object sender, RoutedEventArgs e)
        {
            if (context.ActiveProfile == null) return;

            if (!context.SubscriptionsManager.DeactivateProfile())
            {
                MessageBox.Show("The active profile could not be deactivated, see the log for more details", "Profile failed to deactivate!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void AddProfile(object sender, RoutedEventArgs e)
        {
            var w = new ProfileDialog(context, null);
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;

            ReloadProfileTree();
        }

        private void AddChildProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var w = new ProfileDialog(context, pi.profile);
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;

            ReloadProfileTree();
        }

        private void EditProfile(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
            }
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var win = new ProfileWindow(context, pi.profile);
            Action showAction = () => win.Show();
            Dispatcher.BeginInvoke(showAction);
        }

        private void RenameProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var w = new TextDialog("Rename profile", pi.profile.Title);
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;
            pi.profile.Rename(w.TextResult);
            ReloadProfileTree();
        }

        private void CopyProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var w = new TextDialog("Profile name", pi.profile.Title + " Copy");
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;
            context.ProfilesManager.CopyProfile(pi.profile, w.TextResult);
            ReloadProfileTree();
        }

        private void RemoveProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var result = MessageBox.Show("Are you sure you want to remove '" + pi.profile.Title + "'?", "Remove profile", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            pi.profile.Remove();
            ReloadProfileTree();
        }

        #endregion Profile Actions

        private void ManageDeviceLists_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new DeviceListWindow(context);
            Action showAction = () => win.Show();
            Dispatcher.BeginInvoke(showAction);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (context.IsNotSaved)
            {
                var result = MessageBox.Show("Do you want to save before closing?", "Unsaved data", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.None:
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;

                    case MessageBoxResult.Yes:
                        context.SaveContext();
                        break;

                    case MessageBoxResult.No:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            context.Dispose();
        }

        private void ActiveProfileChanged()
        {
            OnPropertyChanged(nameof(ActiveProfileBreadCrumbs));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Save_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            context.SaveContext();
        }

        private void Save_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = context.IsNotSaved;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != NativeMethods.WM_COPYDATA) return IntPtr.Zero;

            var data = (NativeMethods.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(NativeMethods.COPYDATASTRUCT));
            var argsString = Marshal.PtrToStringAnsi(data.lpData);
            if (!string.IsNullOrEmpty(argsString)) context.ParseCommandLineArguments(argsString.Split(';'));
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

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new AboutWindow();
            Action showAction = () => win.Show();
            Dispatcher.BeginInvoke(showAction);
        }

        private void Help_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new HelpWindow();
            Action showAction = () => win.Show();
            Dispatcher.BeginInvoke(showAction);
        }
    }
}