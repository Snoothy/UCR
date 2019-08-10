using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class ProfileDeviceListControlViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DeviceItem> Devices { get; set; }
        public bool IsRemoveEnabled => CanRemoveDevice(SelectedDevice);
        public bool IsConfigurationEnabled => CanManageDeviceConfiguration();
        private DeviceConfiguration _device;
        public DeviceConfiguration SelectedDevice
        {
            get => _device;
            set
            {
                _device = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRemoveEnabled));
                OnPropertyChanged(nameof(IsConfigurationEnabled));
            }
        }

        private readonly Profile _profile;
        private readonly DeviceIoType _deviceIoType;

        public ProfileDeviceListControlViewModel()
        {
        }

        public ProfileDeviceListControlViewModel(Profile profile, List<DeviceConfiguration> devices, DeviceIoType deviceIoType)
        {
            _profile = profile;
            _deviceIoType = deviceIoType;
            Devices = new ObservableCollection<DeviceItem>();
            foreach (var device in devices)
            {
                Devices.Add(new DeviceItem(device, profile));
            }
        }

        private bool CanRemoveDevice(DeviceConfiguration deviceConfiguration)
        {
            if (deviceConfiguration == null) return false;
            return _profile.CanRemoveDeviceConfiguration(deviceConfiguration);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void RemoveDevice(DeviceItem deviceItem)
        {
            var dialog = new BoolDialog("Remove device", $"Are you sure you want to remove {deviceItem.Title} from {deviceItem.DeviceConfiguration.Device.Profile.Title}?");
            var result = (bool?)await DialogHost.Show(dialog, "RootDialog");
            if (result == null || !result.Value) return;

            _profile.RemoveDeviceConfiguration(deviceItem.DeviceConfiguration);
            Devices.Remove(deviceItem);
            OnPropertyChanged(nameof(Devices));
        }

        public async void AddDevices()
        {
            var deviceList = _profile.GetMissingDeviceList(_deviceIoType);
            var dialog = new AddDevicesDialog(deviceList, _deviceIoType);
            var result = (AddDevicesDialogViewModel)await DialogHost.Show(dialog, "RootDialog");
            if (result?.Devices == null) return;

            var deviceConfigurations = result.Devices.GetSelectedDevices().Select(d => new DeviceConfiguration(d.Device)).ToList();
            _profile.AddDeviceConfigurations(deviceConfigurations, _deviceIoType);
            foreach (var deviceConfiguration in deviceConfigurations)
            {
                Devices.Add(new DeviceItem(deviceConfiguration, _profile));
            }
            OnPropertyChanged(nameof(Devices));
        }

        public void ManageDeviceConfiguration()
        {
            throw new NotImplementedException();
        }

        private bool CanManageDeviceConfiguration()
        {
            if (SelectedDevice == null) return false;
            return true;
        }
    }
}
