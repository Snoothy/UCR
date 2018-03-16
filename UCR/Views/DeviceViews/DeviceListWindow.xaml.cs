using System.Windows;
using HidWizards.UCR.Core;
using HidWizards.UCR.ViewModels.DeviceViewModels;

namespace HidWizards.UCR.Views.DeviceViews
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
