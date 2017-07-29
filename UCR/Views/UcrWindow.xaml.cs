using System.Collections.ObjectModel;
using System.Windows;
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
            InitializeComponent();
            ctx = new UCRContext();

            //var Io = new IOWrapper.IOController();
            //var list = Io.GetInputList();

            ReloadProfileTree();
            ctx.ActivateProfile(ctx.Profiles[0]);
        }

        private void ReloadProfileTree()
        {
            var profileTree = ProfileItem.GetProfileTree(ctx.Profiles);
            ProfileTree.ItemsSource = profileTree;
        }

        private void AddChildProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi = ProfileTree.SelectedItem as ProfileItem;
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
            ctx.Profiles.Add(Profile.CreateProfile(w.TextResult));
            ReloadProfileTree();
            ctx.IsNotSaved = true;
        }

        private void EditProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi = ProfileTree.SelectedItem as ProfileItem;
            ProfileWindow win = new ProfileWindow(ctx, pi.profile);
            win.Show();
        }

        private void RenameProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi = ProfileTree.SelectedItem as ProfileItem;
            var w = new TextDialog("Rename profile", pi.profile.Title);
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;
            pi.profile.Title = w.TextResult;
            ReloadProfileTree();
        }

        private void CopyProfile(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not yet implemented", "We're sorry...", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return;

            ProfileItem pi = ProfileTree.SelectedItem as ProfileItem;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete '" + pi.profile.Title + "'?", "Delete profile", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                pi.profile.Delete(ctx);
                ReloadProfileTree();
            }
        }

        private void DeleteProfile(object sender, RoutedEventArgs e)
        {
            ProfileItem pi = ProfileTree.SelectedItem as ProfileItem;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete '" + pi.profile.Title +"'?", "Delete profile", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                pi.profile.Delete(ctx);
                ReloadProfileTree();
            }
        }

        private void ShowDevices(object sender, RoutedEventArgs e)
        {
            ctx.ActiveProfile.Joysticks.Devices[0].Test();
        }

    }
}
