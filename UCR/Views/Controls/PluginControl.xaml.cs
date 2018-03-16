using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.Views.Controls
{
    public partial class MappingControl : UserControl
    {

        public MappingControl()
        {
            InitializeComponent();
        }

        private void Remove_OnClick(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var mapping = button.DataContext as Mapping;
            //mapping?.Remove();
            // TODO
        }
    }
}
