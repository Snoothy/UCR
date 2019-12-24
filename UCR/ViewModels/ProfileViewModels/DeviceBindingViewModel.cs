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
using HidWizards.UCR.Core.Utilities;

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
        public Visibility ShowPropertyList => PluginPropertyGroup == null ? Visibility.Collapsed : Visibility.Visible;
        public Visibility ShowBlock => DeviceBinding.IsBlockable() ? Visibility.Visible : Visibility.Collapsed;
        public PluginPropertyGroupViewModel PluginPropertyGroup { get; set; }
        public long PreviewValue => GetPreviewValue();
        public bool ShowButtonPreview => DeviceBinding.IsInBindMode || DeviceBinding.Profile.IsActive();

        private bool GuiInvalidated { get; set; }

        private long GetPreviewValue()
        {
            if (DeviceBinding.IsInBindMode)
            {
                return BindModeProgress;
            } else if (DeviceBinding.Profile.IsActive())
            {
                switch (DeviceBindingCategory)
                {
                    case DeviceBindingCategory.Momentary:
                        return 100 * CurrentValue;
                    case DeviceBindingCategory.Range:
                        return (long) (50.0 + ((double) CurrentValue / Constants.AxisMaxValue) * 50);
                    case DeviceBindingCategory.Event:
                    case DeviceBindingCategory.Delta:
                    default:
                        return 0;
                }
            }

            return 0;
        }

        private bool _bindingEnabled;
        public bool BindingEnabled
        {
            get => _bindingEnabled;
            set
            {
                _bindingEnabled = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PreviewValue));
                OnPropertyChanged(nameof(ShowButtonPreview));
            }
        }

        public bool Block
        {
            get => DeviceBinding.Block;
            set => DeviceBinding.SetBlock(value);
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
                if (_currentValue == value) return;
                _currentValue = value;
                GuiInvalidated = true;
                OnPropertyChanged(nameof(ShowButtonPreview));
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
                OnPropertyChanged(nameof(PreviewValue));
                OnPropertyChanged(nameof(ShowButtonPreview));
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
            var deviceConfigurationList = DeviceBinding.Profile.GetDeviceConfigurationList(DeviceBinding.DeviceIoType);
            Devices = new ObservableCollection<ComboBoxItemViewModel>();
            foreach (var deviceConfiguration in deviceConfigurationList)
            {
                Devices.Add(new ComboBoxItemViewModel(deviceConfiguration.GetFullTitleForProfile(DeviceBinding.Profile), deviceConfiguration.Guid));
            }

            SetSelectDevice();
        }

        private void SetSelectDevice()
        {
            ComboBoxItemViewModel selectedDevice = null;

            foreach (var comboBoxItem in Devices)
            {
                if (comboBoxItem.Value == DeviceBinding.DeviceConfigurationGuid)
                {
                    selectedDevice = comboBoxItem;
                    break;
                }
            }

            if (Devices.Count == 0) Devices.Add(new ComboBoxItemViewModel("No devices", Guid.Empty));
            if (selectedDevice == null)
            {
                selectedDevice = Devices[0];
            }

            SelectedDevice = selectedDevice;
        }

        public void CurrentValueChanged()
        {
            if (!GuiInvalidated) return;
            OnPropertyChanged(nameof(CurrentValue));
            OnPropertyChanged(nameof(PreviewValue));
        }

        private void DeviceBindingOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var deviceBinding = (DeviceBinding) sender;
            if (!deviceBinding.Guid.Equals(DeviceBinding.Guid)) return;

            CurrentValue = deviceBinding.CurrentValue;

            if (propertyChangedEventArgs.PropertyName.Equals(nameof(DeviceBinding.IsBound))
                || propertyChangedEventArgs.PropertyName.Equals(nameof(DeviceBinding.IsInBindMode)))
            {
                BindModeProgress = 0;
                OnPropertyChanged(nameof(BindButtonText));
                OnPropertyChanged(nameof(ShowPreview));
                OnPropertyChanged(nameof(ShowBindMode));
            }

            if (propertyChangedEventArgs.PropertyName.Equals(nameof(DeviceBinding.IsBound)))
            {
                SetSelectDevice();
                OnPropertyChanged(nameof(SelectedDevice));
                OnPropertyChanged(nameof(ShowBlock));
                OnPropertyChanged(nameof(Block));
            }
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(DeviceBinding.DeviceConfigurationGuid)))
            {
                OnPropertyChanged(nameof(BindButtonText));
                OnPropertyChanged(nameof(ShowBlock));
                OnPropertyChanged(nameof(Block));
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
