using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels.Controls;
using HidWizards.UCR.ViewModels.DeviceViewModels;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class CreateProfileDialogViewModel
    {

        public string Title { get; set; }
        public string ProfileName { get; set; }
        public DeviceSelectControlViewModel InputControl { get; set; }
        public DeviceSelectControlViewModel OutputControl { get; set; }
        public CreateProfileDialogViewModel ViewModel => this;
        public DevicesManager DevicesManager { get; set; }

        public CreateProfileDialogViewModel()
        {
        }

        public CreateProfileDialogViewModel(string title, DevicesManager devicesManager)
        {
            Title = title;
            DevicesManager = devicesManager;

            var inputDevices = devicesManager.GetAvailableDeviceList(DeviceIoType.Input);
            var outputDevices = devicesManager.GetAvailableDeviceList(DeviceIoType.Output);

           InputControl = new DeviceSelectControlViewModel("Input Devices", inputDevices);
           OutputControl = new DeviceSelectControlViewModel("Output Devices", outputDevices);
        }

        public List<Device> GetInputDevices()
        {
            return GetDevicesFromViewModel(InputControl);
        }

        public List<Device> GetOutputDevices()
        {
            return GetDevicesFromViewModel(OutputControl);
        }

        private List<Device> GetDevicesFromViewModel(DeviceSelectControlViewModel viewModel)
        {
            return viewModel.Devices.Where(d => d.Checked).Select(d => d.Device).ToList();
        }
    }
}
