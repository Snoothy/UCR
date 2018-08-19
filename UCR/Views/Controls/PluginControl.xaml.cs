using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[+-]?([0-9]*[.])?[0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
