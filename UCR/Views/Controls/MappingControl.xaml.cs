using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.ViewModels.ProfileViewModels;

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
            var mappingViewModel = button.DataContext as MappingViewModel;
            mappingViewModel.Remove();
        }

    }
}
