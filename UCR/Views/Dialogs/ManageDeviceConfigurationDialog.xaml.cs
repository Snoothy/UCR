using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels.Dashboard;
using HidWizards.UCR.ViewModels.Dialogs;

namespace HidWizards.UCR.Views.Dialogs
{
    public partial class ManageDeviceConfigurationDialog : UserControl
    {
        private ManageDeviceConfigurationViewModel ViewModel { get; set; }

        public ManageDeviceConfigurationDialog(DeviceConfiguration deviceConfiguration, DeviceIoType deviceIoType)
        {
            ViewModel = new ManageDeviceConfigurationViewModel(deviceConfiguration, deviceIoType);
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
