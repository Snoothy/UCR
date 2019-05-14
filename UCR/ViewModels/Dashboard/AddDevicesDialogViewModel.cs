using System;
using System.Collections.Generic;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels.Controls;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class AddDevicesDialogViewModel
    {

        public DeviceSelectControlViewModel Devices { get; set; }

        public AddDevicesDialogViewModel ViewModel { get; set; }

        public AddDevicesDialogViewModel()
        {
        }

        public AddDevicesDialogViewModel(List<Device> devices, DeviceIoType deviceIoType)
        {
            Devices = new DeviceSelectControlViewModel($"Add {(deviceIoType == DeviceIoType.Input ? "input" : "output")} devices", devices);
            ViewModel = this;
            
        }
    }
}
