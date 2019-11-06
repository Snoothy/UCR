using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HidWizards.UCR.ViewModels.Controls;
using HidWizards.UCR.ViewModels.DeviceViewModels;

namespace HidWizards.UCR.Views.Controls
{
    public partial class DeviceAddRemoveControl : UserControl
    {
        
        public DeviceAddRemoveControl()
        {
            InitializeComponent();
        }

        private void Device_OnAddDevice(object sender, MouseButtonEventArgs e)
        {
            var viewModel = DataContext as DeviceAddRemoveControlViewModel;
            var device = (sender as Grid)?.DataContext as DeviceViewModel;
            viewModel.AddShadowDevice(device);
        }

        private void Device_OnRemoveDevice(object sender, MouseButtonEventArgs e)
        {
            var viewModel = DataContext as DeviceAddRemoveControlViewModel;
            var device = (sender as Grid)?.DataContext as DeviceViewModel;
            viewModel.RemoveShadowDevice(device);
        }
    }
}
