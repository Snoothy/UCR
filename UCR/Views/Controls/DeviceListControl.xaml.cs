using System.Windows;
using System.Windows.Controls;
using UCR.Models.Mapping;
using UCR.ViewModels.Device;

namespace UCR.Views.Controls
{
    /// <summary>
    /// Interaction logic for DeviceListControl.xaml
    /// </summary>
    public partial class DeviceListControl : UserControl
    {
        public DeviceListControl()
        {
            InitializeComponent();
        }

        private void AddDeviceGroup_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as DeviceListControlViewModel;
            var w = new TextDialog("Device group name");
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;
            viewModel.AddDeviceGroup(w.TextResult);
        }
    }
}
