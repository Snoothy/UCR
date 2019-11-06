using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels.Controls;
using HidWizards.UCR.ViewModels.DeviceViewModels;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class ManageDeviceConfigurationViewModel
    {
        public string Title => "Device configuration";

        public string Hint => _deviceConfiguration.Device.Title;

        public string DeviceConfigurationName { get; set; }

        public DeviceAddRemoveControlViewModel ShadowDevices { get; set; }
        public ManageDeviceConfigurationViewModel ViewModel { get; set; }

        public bool HasChanged => _changed;

        private readonly DeviceConfiguration _deviceConfiguration;
        private readonly DeviceIoType _deviceIoType;
        private readonly bool _changed;

        public ManageDeviceConfigurationViewModel()
        {
            _changed = false;
        }

        public ManageDeviceConfigurationViewModel(DeviceConfiguration deviceConfiguration, DeviceIoType deviceIoType)
        {
            ViewModel = this;
            _deviceConfiguration = deviceConfiguration;
            _deviceIoType = deviceIoType;
            DeviceConfigurationName = _deviceConfiguration.ConfigurationName;
            ShadowDevices = new DeviceAddRemoveControlViewModel("Available Devices", "Selected Shadow Devices", GetAllShadowDevices());
            _changed = true;
        }

        public List<Device> GetSelectedShadowDevices()
        {
            return ShadowDevices.ShadowDevices.Select(d => d.Device).ToList();
        }

        private List<DeviceViewModel> GetAllShadowDevices()
        {
            var deviceViewModels = new List<DeviceViewModel>();

            foreach (var shadowDevice in _deviceConfiguration.ShadowDevices)
            {
                deviceViewModels.Add(new DeviceViewModel(shadowDevice, true));
            }

            foreach (var shadowDevice in _deviceConfiguration.getAvailableShadowDevices(_deviceIoType))
            {
                if (ShadowDeviceAlreadySelected(shadowDevice, deviceViewModels)) continue;

                deviceViewModels.Add(new DeviceViewModel(shadowDevice));
            }

            return deviceViewModels;
        }

        private bool ShadowDeviceAlreadySelected(Device shadowDevice, List<DeviceViewModel> list)
        {
            return list.Select(d => d.Device).ToList().Contains(shadowDevice);
        }
    }
}
