using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models.Device;

namespace HidWizards.UCR.ViewModels.Device
{
    public class DeviceListControlViewModel
    {
        public Context context { get; }
        private readonly DeviceIoType _deviceIoType;

        public ObservableCollection<DeviceGroupViewModel> InputDeviceGroups { get; set; }
        public ObservableCollection<DeviceGroupViewModel> OutputDeviceGroups { get; set; }

        public DeviceListControlViewModel(Context context, DeviceIoType deviceIoType) : this()
        {
            this.context = context;
            _deviceIoType = deviceIoType;
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
            var guid = context.DeviceGroupsManager.AddDeviceGroup(title, _deviceIoType);
            OutputDeviceGroups.Add(new DeviceGroupViewModel(title, guid));
        }

        public void RemoveDeviceGroup(DeviceGroupViewModel deviceGroupViewModel)
        {
            var result = MessageBox.Show("Are you sure you want to remove '" + deviceGroupViewModel.Title + "'?", "Remove device group", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            context.DeviceGroupsManager.RemoveDeviceGroup(deviceGroupViewModel.Guid, _deviceIoType);
            OutputDeviceGroups.Remove(deviceGroupViewModel);
        }

        public void RenameDeviceGroup(DeviceGroupViewModel deviceGroup, string title)
        {
            OutputDeviceGroups.First(d => d.Guid == deviceGroup.Guid).Title = title;
            context.DeviceGroupsManager.RenameDeviceGroup(deviceGroup.Guid, _deviceIoType, title);
        }

        public void AddDeviceToDeviceGroup(global::HidWizards.UCR.Core.Models.Device.Device device, Guid deviceGroupGuid)
        {
            context.DeviceGroupsManager.AddDeviceToDeviceGroup(device, _deviceIoType, deviceGroupGuid);
            OutputDeviceGroups.First(d => d.Guid == deviceGroupGuid).Devices.Add(device);
        }

        public void RemoveDeviceFromDeviceGroup(global::HidWizards.UCR.Core.Models.Device.Device device)
        {
            var deviceGroupViewModel = DeviceGroupViewModel.FindDeviceGroupViewModelWithDevice(OutputDeviceGroups, device);
            context.DeviceGroupsManager.RemoveDeviceFromDeviceGroup(device, _deviceIoType, deviceGroupViewModel.Guid);
            deviceGroupViewModel.Devices.Remove(device);
        }

        private void GenerateDeviceList()
        {
            // TODO Generate list based on deviceIOtype
            switch (_deviceIoType)
            {
                case DeviceIoType.Input:
                    foreach (var deviceGroup in PopulateDeviceList(context.DevicesManager.GetAvailableDeviceList(DeviceIoType.Input), "Input devices").Groups)
                    {
                        InputDeviceGroups.Add(deviceGroup);
                    }
                    break;
                case DeviceIoType.Output:
                    foreach (var deviceGroup in PopulateDeviceList(context.DevicesManager.GetAvailableDeviceList(DeviceIoType.Output), "Output devices").Groups)
                    {
                        InputDeviceGroups.Add(deviceGroup);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private DeviceGroupViewModel PopulateDeviceList(List<DeviceGroup> deviceGroups, string title)
        {
            var result = new DeviceGroupViewModel()
            {
                Title = title
            };
            foreach (var deviceGroup in deviceGroups)
            {
                var deviceGroupViewModel = new DeviceGroupViewModel()
                {
                    Title = deviceGroup.Title
                };
                foreach (var device in deviceGroup.Devices)
                {
                    deviceGroupViewModel.Devices.Add(device);
                }
                result.Groups.Add(deviceGroupViewModel);
            }
            return result;
        }

        private void GenerateDeviceGroupList()
        {
            List<DeviceGroup> deviceList;
            switch (_deviceIoType)
            {
                case DeviceIoType.Input:
                    deviceList = context.InputGroups;
                    break;
                case DeviceIoType.Output:
                    deviceList = context.OutputGroups;
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
