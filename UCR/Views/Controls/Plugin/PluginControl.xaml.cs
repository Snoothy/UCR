using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.ViewModels.ProfileViewModels;

namespace HidWizards.UCR.Views.Controls.Plugin
{
    public partial class PluginControl : UserControl
    {

        public PluginControl()
        {
            InitializeComponent();
        }

        private void RemovePlugin_OnClick(object sender, RoutedEventArgs e)
        {
            var plugin = DataContext as PluginViewModel;
            plugin?.Remove();
        }
    }
}
