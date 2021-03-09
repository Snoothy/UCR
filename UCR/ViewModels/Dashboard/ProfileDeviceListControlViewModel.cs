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
        public bool IsRemoveEnabled => CanRemoveDevice();
        public bool IsConfigurationEnabled => CanManageDeviceConfiguration();
        private DeviceItem _deviceConfiguration;
        public DeviceItem SelectedDeviceConfiguration
        {
            get => _deviceConfiguration;
            set
            {
                _deviceConfiguration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRemoveEnabled));
                OnPropertyChanged(nameof(IsConfigurationEnabled));
            }
        }

        private List<DeviceItem> _devicesConfigurations;
        public List<DeviceItem> SelectedDevicesConfigurations
        {
            get => _devicesConfigurations;
            set
            {
                _devicesConfigurations = value;
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

        private bool CanRemoveDevice()
        {
            if (SelectedDevicesConfigurations == null || !SelectedDevicesConfigurations.Any()) return false;

            return _profile.CanRemoveDeviceConfiguration(
                SelectedDevicesConfigurations.Select(_ => _.DeviceConfiguration));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void RemoveDevices(IEnumerable<DeviceItem> deviceItems)
        {
            var profiles = deviceItems.GroupBy(_ => _.DeviceConfiguration.Device.Profile).ToArray();

            if (profiles.Length > 1)
                return;

            foreach (var profile in profiles) {
                var text = Environment.NewLine
                         + Environment.NewLine
                         + string.Join(Environment.NewLine, profile.Select(_ => _.Title))
                         + Environment.NewLine
                         + Environment.NewLine;

                var dialog = new BoolDialog("Remove device",
                    $"Are you sure you want to remove {text}from [{profile.Key.Title}] profile?");

                var result = (bool?) await DialogHost.Show(dialog, "RootDialog");

                if (result == null || !result.Value)
                    return;

                foreach (var deviceItem in profile) {
                    _profile.RemoveDeviceConfiguration(deviceItem.DeviceConfiguration);
                    Devices.Remove(deviceItem);
                }
            }

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

        public async void ManageDeviceConfiguration()
        {
            var dialog = new ManageDeviceConfigurationDialog(SelectedDeviceConfiguration.DeviceConfiguration, _deviceIoType);
            var result = (ManageDeviceConfigurationViewModel)await DialogHost.Show(dialog, "RootDialog");
            if (result == null || !result.HasChanged) return;

            SelectedDeviceConfiguration.DeviceConfiguration.ChangeConfigurationName(result.DeviceConfigurationName);
            SelectedDeviceConfiguration.DeviceConfiguration.ChangeShadowDevices(result.GetSelectedShadowDevices());

            SelectedDeviceConfiguration.TitleChanged();
            OnPropertyChanged(nameof(Devices));
        }

        private bool CanManageDeviceConfiguration()
        {
            if (SelectedDeviceConfiguration == null) return false;
            return true;
        }
    }
}
