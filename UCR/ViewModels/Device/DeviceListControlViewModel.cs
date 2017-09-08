using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Providers;
using UCR.Models;
using UCR.Models.Devices;

namespace UCR.ViewModels.Device
{
    public class DeviceListControlViewModel
    {
        public UCRContext ctx { get; }
        private readonly DeviceType _deviceType;

        public ObservableCollection<DeviceGroupViewModel> InputDeviceGroups { get; set; }
        public ObservableCollection<DeviceGroupViewModel> OutputDeviceGroups { get; set; }

        public DeviceListControlViewModel(UCRContext ctx, DeviceType deviceType) : this()
        {
            this.ctx = ctx;
            _deviceType = deviceType;
            GenerateDeviceList();
            GenerateDeviceGroupList();
        }

        public DeviceListControlViewModel()
        {
            InputDeviceGroups = new ObservableCollection<DeviceGroupViewModel>();
            OutputDeviceGroups = new ObservableCollection<DeviceGroupViewModel>();
        }

        public void AddDeviceGroup(string title)
        {
            var guid = ctx.AddDeviceGroup(title, _deviceType);
            OutputDeviceGroups.Add(new DeviceGroupViewModel(title, guid));
        }

        public void RemoveDeviceGroup(DeviceGroupViewModel deviceGroupViewModel)
        {
            var result = MessageBox.Show("Are you sure you want to remove '" + deviceGroupViewModel.Title + "'?", "Remove device group", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            ctx.RemoveDeviceGroup(deviceGroupViewModel.Guid, _deviceType);
            OutputDeviceGroups.Remove(deviceGroupViewModel);
        }

        public void RenameDeviceGroup(DeviceGroupViewModel deviceGroup, string title)
        {
            OutputDeviceGroups.First(d => d.Guid == deviceGroup.Guid).Title = title;
            ctx.RenameDeviceGroup(deviceGroup.Guid, _deviceType, title);
        }

        public void AddDeviceToDeviceGroup(Models.Devices.Device device, Guid deviceGroupGuid)
        {
            ctx.AddDeviceToDeviceGroup(device, _deviceType, deviceGroupGuid);
            OutputDeviceGroups.First(d => d.Guid == deviceGroupGuid).Devices.Add(device);
        }

        public void RemoveDeviceFromDeviceGroup(Models.Devices.Device device)
        {
            var deviceGroupViewModel = DeviceGroupViewModel.FindDeviceGroupViewModelWithDevice(OutputDeviceGroups, device);
            ctx.RemoveDeviceFromDeviceGroup(device, _deviceType, deviceGroupViewModel.Guid);
            deviceGroupViewModel.Devices.Remove(device);
        }

        private void GenerateDeviceList()
        {
            InputDeviceGroups.Add(PopulateDeviceList(ctx.IOController.GetInputList(), "Input devices"));
            InputDeviceGroups.Add(PopulateDeviceList(ctx.IOController.GetOutputList(), "Output devices"));
        }

        private DeviceGroupViewModel PopulateDeviceList(SortedDictionary<string, ProviderReport> list, string title)
        {
            var result = new DeviceGroupViewModel()
            {
                Title = title
            };
            foreach (var providerReport in list)
            {
                var deviceGroupViewModel = new DeviceGroupViewModel()
                {
                    Title = providerReport.Key
                };
                foreach (var ioWrapperDevice in providerReport.Value.Devices)
                {
                    deviceGroupViewModel.Devices.Add(new Models.Devices.Device(ioWrapperDevice.Value));
                }
                result.Groups.Add(deviceGroupViewModel);
            }
            return result;
        }

        private void GenerateDeviceGroupList()
        {
            List<DeviceGroup> deviceList;
            switch (_deviceType)
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
                var deviceGroupViewModel = new DeviceGroupViewModel(deviceGroup);
                OutputDeviceGroups.Add(deviceGroupViewModel);
            }
        }
    }
}
