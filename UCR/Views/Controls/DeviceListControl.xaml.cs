using System;
using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.ViewModels.DeviceViewModels;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Views.Dialogs;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace HidWizards.UCR.Views.Controls
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
            var device = (sender as MenuItem)?.DataContext as Core.Models.Device
                         ?? DeviceTreeView.SelectedItem as Core.Models.Device;
            if (device == null)
            {
                MessageBox.Show("Please select a device", "No device selected!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var deviceGroup = DeviceGroupTreeView.SelectedItem as DeviceGroupViewModel;
            if (deviceGroup == null)
            {
                var outputDevice = DeviceGroupTreeView.SelectedItem as Core.Models.Device;
                if (outputDevice != null)
                {
                    deviceGroup = DeviceGroupViewModel.FindDeviceGroupViewModelWithDevice(GetViewModel().OutputDeviceGroups, outputDevice);
                }
                else
                {
                    MessageBox.Show("Please select a device group", "No device group selected!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            GetViewModel().AddDeviceToDeviceGroup(device, deviceGroup.Guid);
        }

        private void RemoveDevice_OnClick(object sender, RoutedEventArgs e)
        {
            // Select device from context menu first, then default to selected item
            var device = (sender as MenuItem)?.DataContext as Core.Models.Device
                ?? DeviceGroupTreeView.SelectedItem as Core.Models.Device;
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

        private void RemoveDeviceGroup_OnClick(object sender, RoutedEventArgs e)
        {
            var deviceGroup = (sender as MenuItem)?.DataContext as DeviceGroupViewModel;
            GetViewModel().RemoveDeviceGroup(deviceGroup);
        }

        private void RenameDeviceGroup_OnClick(object sender, RoutedEventArgs e)
        {
            var deviceGroup = (sender as MenuItem)?.DataContext as DeviceGroupViewModel;
            if (deviceGroup == null) return;
            var w = new TextDialog("Rename device group", deviceGroup.Title);
            w.ShowDialog();
            if (!w.DialogResult.HasValue || !w.DialogResult.Value) return;
            GetViewModel().RenameDeviceGroup(deviceGroup, w.TextResult);
        }
    }
}
