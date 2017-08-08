using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using UCR.Models;
using UCR.ViewModels;

namespace UCR.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UCRContext ctx;

        public MainWindow()
        {
            InitResources();
            InitializeComponent();
            ctx = new UCRContext();
            ReloadProfileTree();

            ctx.ActivateProfile(ctx.Profiles[0]); // TODO Mock data
        }

        private void InitResources()
        {
            // TODO Load all resourecs dynamicly
            var foo = new Uri("pack://application:,,,/UCR;component/Views/Plugins/ButtonToButton.xaml");
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = foo });
            foo = new Uri("pack://application:,,,/UCR;component/Views/Plugins/ButtonToAxis.xaml");
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = foo });
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
            var profileTree = ProfileItem.GetProfileTree(ctx.Profiles);
            ProfileTree.ItemsSource = profileTree;
        }

        private void AddChildProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var w = new TextDialog("Profile name");
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;
            pi.profile.AddNewChildProfile(w.TextResult);
            ReloadProfileTree();
            ctx.IsNotSaved = true;
        }

        private void AddProfile(object sender, RoutedEventArgs e)
        {
            var w = new TextDialog("Profile name");
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;
            ctx.Profiles.Add(Profile.CreateProfile(ctx, w.TextResult));
            ReloadProfileTree();
            ctx.IsNotSaved = true;
        }

        private void EditProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var win = new ProfileWindow(ctx, pi.profile);
            win.Show();
        }

        private void RenameProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var w = new TextDialog("Rename profile", pi.profile.Title);
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;
            pi.profile.Title = w.TextResult;
            ReloadProfileTree();
        }

        private void CopyProfile(object sender, RoutedEventArgs e)
        {
            // TODO: Implement
            MessageBox.Show("Not yet implemented", "We're sorry...", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return;

            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete '" + pi.profile.Title + "'?", "Delete profile", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                pi.profile.Delete();
                ReloadProfileTree();
            }
        }

        private void DeleteProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi;
            if (!GetSelectedItem(out pi)) return;
            var result = MessageBox.Show("Are you sure you want to delete '" + pi.profile.Title +"'?", "Delete profile", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            pi.profile.Delete();
            ReloadProfileTree();
        }

        private void ShowDevices(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        // TODO Fix
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (ctx.IsNotSaved)
            {
                var result = MessageBox.Show("Do you want to save before closing?", "Unsaved data", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.None:
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                    case MessageBoxResult.Yes:
                        // TODO save everything
                        break;
                    case MessageBoxResult.No:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // TODO Dispose backend
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            ctx.IOController = null;
        }
    }
}
