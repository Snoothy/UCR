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
using UCR.Views.ProfileViews;
using DeviceListWindow = HidWizards.UCR.Views.DeviceViews.DeviceListWindow;
using ProfileWindow = HidWizards.UCR.Views.ProfileViews.ProfileWindow;

namespace HidWizards.UCR.Views
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Context _context;

        public string ActiveProfileBreadCrumbs => _context?.ActiveProfile != null ? _context.ActiveProfile.ProfileBreadCrumbs() : "None";

        public MainWindow(Context context)
        {
            DataContext = this;
            this._context = context;
            InitializeComponent();
            
            context.SetActiveProfileCallback(ActiveProfileChanged);
            ReloadProfileTree();
        }

        /// <inheritdoc />
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
                MessageBox.Show("Please select a profile", "No profile selected!",MessageBoxButton.OK, MessageBoxImage.Exclamation);
                profileItem = null;
                return false;
            }
            profileItem = pi;
            return true;
        }

        private void ReloadProfileTree()
        {
            var profileTree = ProfileItem.GetProfileTree(_context.Profiles);
            ProfileTree.ItemsSource = profileTree;
        }


        #region Profile Actions

        private void ActivateProfile(object sender, RoutedEventArgs e)
        {
            var a = sender as ProfileItem;
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            if (!_context.SubscriptionsManager.ActivateProfile(pi.profile))
            {
                MessageBox.Show("The profile could not be activated, see the log for more details", "Profile failed to activate!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DeactivateProfile(object sender, RoutedEventArgs e)
        {
            if (_context.ActiveProfile == null) return;
            
            if (!_context.SubscriptionsManager.DeactivateProfile())
            {
                MessageBox.Show("The active profile could not be deactivated, see the log for more details", "Profile failed to deactivate!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void AddProfile(object sender, RoutedEventArgs e)
        {
            var w = new ProfileDialog(_context, null);
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;

            ReloadProfileTree();
        }

        private void AddChildProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var w = new ProfileDialog(_context, pi.profile);
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
            var win = new ProfileWindow(_context, pi.profile);
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
            _context.ProfilesManager.CopyProfile(pi.profile, w.TextResult);
            ReloadProfileTree();
        }

        private void RemoveProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var result = MessageBox.Show("Are you sure you want to remove '" + pi.profile.Title +"'?", "Remove profile", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            pi.profile.Remove();
            ReloadProfileTree();
        }

        #endregion Profile Actions

        private void ManageDeviceLists_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new DeviceListWindow(_context);
			void ShowAction() => win.Show();
			Dispatcher.BeginInvoke((Action) ShowAction);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (_context.IsNotSaved)
            {
                var result = MessageBox.Show("Do you want to save before closing?", "Unsaved data", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.None:
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                    case MessageBoxResult.Yes:
                        _context.SaveContext();
                        break;
                    case MessageBoxResult.No:
                        break;
					case MessageBoxResult.OK:
						break;
					default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            _context.Dispose();
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
            _context.SaveContext();
        }

        private void Save_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _context.IsNotSaved;
        }
		
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != NativeMethods.WM_COPYDATA) return IntPtr.Zero;
            
            var data = (NativeMethods.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(NativeMethods.COPYDATASTRUCT));
            var argsString = Marshal.PtrToStringAnsi(data.lpData);
            if (!string.IsNullOrEmpty(argsString)) _context.ParseCommandLineArguments(argsString.Split(';'));
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
			void ShowAction() => win.Show();
			Dispatcher.BeginInvoke((Action) ShowAction);
        }

        private void Help_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new HelpWindow();
			void ShowAction() => win.Show();
			Dispatcher.BeginInvoke((Action) ShowAction);
        }
    }
}