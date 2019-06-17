using System;
using System.Collections.Generic;
using System.Windows.Controls;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels.Dashboard;

namespace HidWizards.UCR.Views.Dialogs
{
    public partial class AddDevicesDialog : UserControl
    {
        public AddDevicesDialog(List<Device> devices, DeviceIoType deviceIoType)
        {
            DataContext = new AddDevicesDialogViewModel(devices, deviceIoType);

            InitializeComponent();
        }
    }
}
