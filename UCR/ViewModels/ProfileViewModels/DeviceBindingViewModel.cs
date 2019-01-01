using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class DeviceBindingViewModel : INotifyPropertyChanged
    {
        public string DeviceBindingName { get; set; }
        public string IoTypeName => DeviceBinding.DeviceIoType.Equals(DeviceIoType.Input) ? "Input" : "Output";
        public DeviceBindingCategory DeviceBindingCategory { get; set; }
        public ObservableCollection<ComboBoxItemViewModel> Devices { get; set; }
        public ComboBoxItemViewModel SelectedDevice { get; set; }
        public Visibility ShowPreview => DeviceBinding.IsInBindMode ? Visibility.Hidden : Visibility.Visible;
        public Visibility ShowBindMode => ShowPreview.Equals(Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;

        private bool _bindingEnabled;
        public bool BindingEnabled
        {
            get => _bindingEnabled;
            set
            {
                _bindingEnabled = value;
                OnPropertyChanged();
            }
        }

        public string BindButtonText
        {
            get
            {
                if (DeviceBinding.IsInBindMode) return "Press input device";
                if (DeviceBinding.IsBound) return DeviceBinding.BoundName();
                return "Click to bind";
            }
        }

        private DeviceBinding _deviceBinding;
        public DeviceBinding DeviceBinding
        {
            get => _deviceBinding;
            set
            {
                _deviceBinding = value;
                _deviceBinding.PropertyChanged += DeviceBindingOnPropertyChanged;
                CurrentValue = _deviceBinding.CurrentValue;
            }
        }

        private long _currentValue;
        public long CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChanged();
            }
        }

        private long _bindModeProgress;
        public long BindModeProgress
        {
            get => _bindModeProgress;
            set
            {
                _bindModeProgress = value;
                OnPropertyChanged();
            }
        }

        public DeviceBindingViewModel(DeviceBinding deviceBinding)
        {
            DeviceBinding = deviceBinding;
            deviceBinding.Profile.Context.BindingManager.PropertyChanged += BindingManagerOnPropertyChanged;
            deviceBinding.Profile.Context.SubscriptionsManager.PropertyChanged += SubscriptionsManagerOnPropertyChanged;
            BindingEnabled = !DeviceBinding.Profile.Context.SubscriptionsManager.ProfileActive;
            LoadDeviceInputs();
        }
        
        public void LoadDeviceInputs()
        {
            var devicelist = DeviceBinding.Profile.GetDeviceList(DeviceBinding);
            Devices = new ObservableCollection<ComboBoxItemViewModel>();
            foreach (var device in devicelist)
            {
                Devices.Add(new ComboBoxItemViewModel(device.Title, device.Guid));
            }

            SetSelectDevice();
        }

        private void SetSelectDevice()
        {
            ComboBoxItemViewModel selectedDevice = null;

            foreach (var comboBoxItem in Devices)
            {
                if (comboBoxItem.Value == DeviceBinding.DeviceGuid)
                {
                    selectedDevice = comboBoxItem;
                    break;
                }
            }

            if (Devices.Count == 0) Devices.Add(new ComboBoxItemViewModel("No devices", Guid.Empty));
            if (selectedDevice == null)
            {
                selectedDevice = Devices[0];
                
                // TODO Does bind mode work without this?
                // This incorrectly declares context as changed
                //DeviceBinding.SetDeviceGuid(selectedDevice.Value);
            }

            SelectedDevice = selectedDevice;
        }

        private void DeviceBindingOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var deviceBinding = (DeviceBinding) sender;
            if (!deviceBinding.Guid.Equals(DeviceBinding.Guid)) return;

            CurrentValue = deviceBinding.CurrentValue;

            if (propertyChangedEventArgs.PropertyName.Equals("IsBound")
                || propertyChangedEventArgs.PropertyName.Equals("IsInBindMode"))
            {
                OnPropertyChanged(nameof(BindButtonText));
                OnPropertyChanged(nameof(ShowPreview));
                OnPropertyChanged(nameof(ShowBindMode));
            }

            if (propertyChangedEventArgs.PropertyName.Equals("IsBound"))
            {
                SetSelectDevice();
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }
        
        private void BindingManagerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var bindingManger = sender as BindingManager;
            BindModeProgress = (long)bindingManger.BindModeProgress;
        }

        private void SubscriptionsManagerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals("ProfileActive"))
            {
                BindingEnabled = !DeviceBinding.Profile.Context.SubscriptionsManager.ProfileActive;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
