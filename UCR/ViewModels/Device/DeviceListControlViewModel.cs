using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UCR.Models;
using UCR.Models.Devices;

namespace UCR.ViewModels.Device
{
    public class DeviceListControlViewModel
    {
        private UCRContext ctx;
        private DeviceType deviceType;

        public ObservableCollection<DeviceGroupViewModel> InputDeviceGroups { get; set; }
        public ObservableCollection<DeviceGroupViewModel> OutputDeviceGroups { get; set; }

        public DeviceListControlViewModel(UCRContext ctx, DeviceType deviceType) : this()
        {
            this.ctx = ctx;
            this.deviceType = deviceType;
            GenerateInputList();
            GenerateOutputList();
        }

        public DeviceListControlViewModel()
        {
            InputDeviceGroups = new ObservableCollection<DeviceGroupViewModel>();
            OutputDeviceGroups = new ObservableCollection<DeviceGroupViewModel>();
        }

        private void GenerateInputList()
        {
            foreach (var providerReport in ctx.IOController.GetInputList())
            {
                var deviceGroupViewModel = new DeviceGroupViewModel()
                {
                    Name = providerReport.Key
                };
                foreach (var ioWrapperDevice in providerReport.Value.Devices)
                {
                    deviceGroupViewModel.Devices.Add(new Models.Devices.Device(ioWrapperDevice.Value));
                }
                InputDeviceGroups.Add(deviceGroupViewModel);
            }
        }

        private void GenerateOutputList()
        {
            List<DeviceGroup> deviceList;
            switch (deviceType)
            {
                case DeviceType.Joystick:
                    deviceList = ctx.JoystickGroups;
                    break;
                case DeviceType.Keyboard:
                    deviceList = ctx.KeyboardGroups;
                    break;
                case DeviceType.Mouse:
                    deviceList = ctx.MiceGroups;
                    break;
                case DeviceType.Generic:
                    deviceList = ctx.GenericDeviceGroups;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var deviceGroup in deviceList)
            {
                var deviceGroupViewModel = new DeviceGroupViewModel()
                {
                    Name = deviceGroup.Title,
                    Devices = deviceGroup.Devices
                };
                OutputDeviceGroups.Add(deviceGroupViewModel);
            }
        }
    }
}
