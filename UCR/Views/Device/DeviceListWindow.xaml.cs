using System.Windows;
using UCR.Models;
using UCR.ViewModels.Device;

namespace UCR.Views.Device
{
    /// <summary>
    /// Interaction logic for DeviceListWindow.xaml
    /// </summary>
    public partial class DeviceListWindow : Window
    {
        private DeviceListWindowViewModel viewModel;

        public DeviceListWindow(UCRContext ctx)
        {
            viewModel = new DeviceListWindowViewModel(ctx);
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
