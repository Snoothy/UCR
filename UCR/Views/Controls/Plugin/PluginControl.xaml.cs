using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.ViewModels.ProfileViewModels;
using MaterialDesignThemes.Wpf;

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

        private void AddFilter_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as PluginViewModel;
            viewModel?.AddFilter();
        }

        private void Filter_OnClick(object sender, RoutedEventArgs e)
        {
            var filter = ((Chip)sender).DataContext as FilterViewModel;
            filter?.ToggleFilter();
        }

        private void Filter_OnDeleteClick(object sender, RoutedEventArgs e)
        {
            var filter = ((Chip)sender).DataContext as FilterViewModel;
            filter?.RemoveFilter();
        }
    }
}
