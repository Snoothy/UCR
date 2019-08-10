using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.ViewModels.Dashboard;

namespace HidWizards.UCR.Views.Controls
{
    public partial class ProfileDeviceListControl : UserControl
    {
        public string Title { get; set; }

        public ProfileDeviceListControl()
        {
            InitializeComponent();
        }

        private void AddDevice_OnClick(object sender, RoutedEventArgs e)
        {
            GetViewModel().AddDevices();
        }

        private void RemoveDevice_OnClick(object sender, RoutedEventArgs e)
        {
             GetViewModel().RemoveDevice(GetSelectedDevice());
        }

        private void DeviceList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = GetViewModel();
            if (viewModel == null) return;

            viewModel.SelectedDeviceConfiguration = GetSelectedDevice();
        }

        private DeviceItem GetSelectedDevice()
        {
            return (DeviceItem) DeviceList.SelectedItem;
        }

        private ProfileDeviceListControlViewModel GetViewModel()
        {
            return DataContext as ProfileDeviceListControlViewModel;
        }

        private void ManageDeviceConfiguration_OnClick(object sender, RoutedEventArgs e)
        {
            GetViewModel().ManageDeviceConfiguration();
        }
    }
}
