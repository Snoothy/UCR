using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace HidWizards.UCR.Views.Controls.Plugin
{
    public partial class PluginPropertyListControl : UserControl
    {
        public PluginPropertyListControl()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"([-+0-9]|\.)+");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
