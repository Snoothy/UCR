using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HidWizards.UCR.ViewModels.ProfileViewModels;

namespace HidWizards.UCR.Views.Controls
{
    public partial class PluginControl : UserControl
    {

        public PluginControl()
        {
            InitializeComponent();
        }

        private void RemovePlugin_OnClick(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var plugin = button.DataContext as PluginViewModel;
            plugin?.Remove();
        }
    }
}
