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
        private Device _device;
        public Device SelectedDevice
        {
            get => _device;
            set
            {
                _device = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRemoveEnabled));
            }
        }

        private Profile _profile;
        private DeviceIoType _deviceIoType;

        public ProfileDeviceListControlViewModel()
        {
        }

        public ProfileDeviceListControlViewModel(Profile profile, List<Device> devices, DeviceIoType deviceIoType)
        {
            _profile = profile;
            _deviceIoType = deviceIoType;
            Devices = new ObservableCollection<DeviceItem>();
            foreach (var device in devices)
            {
                Devices.Add(new DeviceItem(device, profile));
            }
        }

        private bool CanRemoveDevice(Device device)
        {
            if (device == null) return false;
            return _profile.CanRemoveDevice(device);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void RemoveDevice(DeviceItem deviceItem)
        {
            var dialog = new BoolDialog("Remove device", $"Are you sure you want to remove {deviceItem.Device.Title} from {deviceItem.Device.Profile.Title}?");
            var result = (bool?)await DialogHost.Show(dialog, "RootDialog");
            if (result == null || !result.Value) return;

            _profile.RemoveDevice(deviceItem.Device);
            Devices.Remove(deviceItem);
            OnPropertyChanged(nameof(Devices));
        }

        public async void AddDevices()
        {
            var deviceList = _profile.GetMissingDeviceList(_deviceIoType);
            var dialog = new AddDevicesDialog(deviceList, _deviceIoType);
            var result = (AddDevicesDialogViewModel)await DialogHost.Show(dialog, "RootDialog");
            if (result?.Devices == null) return;

            _profile.AddDevices(result.Devices.GetSelectedDevices().Select(d => d.Device).ToList(), _deviceIoType);
            foreach (var deviceViewModel in result.Devices.GetSelectedDevices())
            {
                Devices.Add(new DeviceItem(deviceViewModel.Device, _profile));
            }
            OnPropertyChanged(nameof(Devices));
        }
    }
}
