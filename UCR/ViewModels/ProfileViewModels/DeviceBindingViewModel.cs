using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class DeviceBindingViewModel : INotifyPropertyChanged
    {
        public string DeviceBindingName { get; set; }
        public DeviceBindingCategory DeviceBindingCategory { get; set; }

        private DeviceBinding _deviceBinding;
        public DeviceBinding DeviceBinding
        {
            get => _deviceBinding;
            set
            {
                _deviceBinding = value;
                _deviceBinding.PropertyChanged += DeviceBindingOnPropertyChanged;
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

        private void DeviceBindingOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var deviceBinding = (DeviceBinding) sender;
            CurrentValue = deviceBinding.CurrentValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
