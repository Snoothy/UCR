using System.Windows;
using System.Windows.Controls;
using UCR.Core.Models.Plugin;

namespace UCR.Views.Controls
{
    public partial class PluginView : UserControl
    {
        public PluginView()
        {
            InitializeComponent();
        }

        private void RemovePlugin_OnClick(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var plugin = button.DataContext as Plugin;
            plugin?.Remove();
        }
    }
}
