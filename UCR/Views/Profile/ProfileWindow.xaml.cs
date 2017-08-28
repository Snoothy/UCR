using System.Windows;
using UCR.Models;
using UCR.Models.Plugins;

namespace UCR.Views.Profile
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private UCRContext ctx { get; set; }
        private Models.Profile Profile { get; set; }

        public ProfileWindow(UCRContext ctx, Models.Profile profile)
        {
            this.ctx = ctx;
            Profile = profile;
            InitializeComponent();
            Title = "Edit " + profile.Title;
            DataContext = Profile;
        }


        private void ActivateProfile(object sender, RoutedEventArgs e)
        {
            Profile.ctx.ActivateProfile(Profile);
        }

        private void AddPlugin_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new PluginDialog("Add plugin", "Untitled");
            win.ShowDialog();
            if (!win.DialogResult.HasValue || !win.DialogResult.Value) return;
            // TODO Check if plugin with same name exists
            Profile.AddPlugin(win.Plugin, win.TextResult);
            ctx.IsNotSaved = true;
            PluginsListBox.Items.Refresh();
            PluginsListBox.SelectedIndex = PluginsListBox.Items.Count - 1;
            PluginsListBox.ScrollIntoView(PluginsListBox.SelectedItem);
        }

        private void RenamePlugin_OnClick(object sender, RoutedEventArgs e)
        {
            Plugin plugin;
            if (!GetSelectedItem(out plugin)) return;
            var win = new TextDialog("Rename plugin", ((Plugin)PluginsListBox.SelectedItem).Title);
            win.ShowDialog();
            if (!win.DialogResult.HasValue || !win.DialogResult.Value) return;
            // TODO Check if plugin with same name exists
            plugin.Rename(win.TextResult);
            ctx.IsNotSaved = true;

            PluginsListBox.Items.Refresh();
            PluginsListBox.ScrollIntoView(PluginsListBox.SelectedItem);
        }

        private void RemovePlugin_OnClick(object sender, RoutedEventArgs e)
        {
            Plugin plugin;
            if (!GetSelectedItem(out plugin)) return;
            // TODO Check if plugin with same name exists
            Profile.RemovePlugin(plugin);
            ctx.IsNotSaved = true;
            PluginsListBox.Items.Refresh();
            PluginsListBox.ScrollIntoView(PluginsListBox.SelectedItem);
        }

        private bool GetSelectedItem(out Plugin selection)
        {
            var item = PluginsListBox.SelectedItem as Plugin;
            if (item == null)
            {
                MessageBox.Show("Please select a plugin", "No plugin selected!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                selection = null;
                return false;
            }
            selection = item;
            return true;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
