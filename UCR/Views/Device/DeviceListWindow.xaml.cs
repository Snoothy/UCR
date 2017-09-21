using System.Windows;
using UCR.Core;
using UCR.ViewModels.Device;

namespace UCR.Views.Device
{
    /// <summary>
    /// Interaction logic for DeviceListWindow.xaml
    /// </summary>
    public partial class DeviceListWindow : Window
    {
        private DeviceListWindowViewModel viewModel;

        public DeviceListWindow(Context context)
        {
            viewModel = new DeviceListWindowViewModel(context);
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
