using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UCR.Models.Mapping;
using UCR.ViewModels.Device;
using MessageBox = System.Windows.MessageBox;
using TreeView = System.Windows.Forms.TreeView;
using UserControl = System.Windows.Controls.UserControl;

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
            var w = new TextDialog("Device group name");
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;
            GetViewModel().AddDeviceGroup(w.TextResult);
        }

        private void AddDevice_OnClick(object sender, RoutedEventArgs e)
        {

            var device = DeviceTreeView.SelectedItem as Models.Devices.Device;
            if (device == null)
            {
                MessageBox.Show("Please select a device", "No device selected!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var deviceGroup = DeviceGroupTreeView.SelectedItem as DeviceGroupViewModel;
            if (deviceGroup == null)
            {
                MessageBox.Show("Please select a device group", "No device group selected!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            GetViewModel().AddDeviceToDeviceGroup(device, deviceGroup.Guid);
        }

        private void RemoveDevice_OnClick(object sender, RoutedEventArgs e)
        {
            var device = DeviceGroupTreeView.SelectedItem as Models.Devices.Device;
            if (device == null)
            {
                MessageBox.Show("Please select a device to remove", "No device selected!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            GetViewModel().RemoveDeviceFromDeviceGroup(device);
        }

        private DeviceListControlViewModel GetViewModel()
        {
            var viewModel = DataContext as DeviceListControlViewModel;
            if (viewModel == null) throw new ArgumentException("DeviceListControl view model not set in data context");
            return viewModel;
        }

        public static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            current = VisualTreeHelper.GetParent(current);

            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            };
            return null;
        }
    }
}
