using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HidWizards.UCR.Core.Models.Plugin;
using HidWizards.UCR.Views.Profile;

namespace HidWizards.UCR.Views.Controls
{
    public partial class PluginGroupView : ResourceDictionary
    {
        public PluginGroupView()
        {
            InitializeComponent();
            
        }

        private void AddPluginButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = ((Button) sender);
            var pluginGroup = button.DataContext as PluginGroup;
            if (pluginGroup == null) return;
            var win = new PluginDialog(pluginGroup.ParentProfile.context, "Add plugin", "Untitled");
            win.ShowDialog();
            if (!win.DialogResult.HasValue || !win.DialogResult.Value) return;
            pluginGroup.AddPlugin(win.Plugin, win.TextResult);

            // TODO: Do a proper refresh of ListBox
            foreach (var listbox in FindVisualChildren<ListBox>(((Grid)button.Parent).Parent))
            {
                if (listbox.Name != "PluginsListBox") continue;
                listbox.Items.Refresh();
                break;
            }
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }
    }
}