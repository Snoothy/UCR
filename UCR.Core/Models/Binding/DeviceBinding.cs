using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using HidWizards.IOWrapper.DataTransferObjects;
using HidWizards.UCR.Core.Annotations;

namespace HidWizards.UCR.Core.Models.Binding
{
    public enum DeviceBindingCategory
    {
        Event,
        Momentary,
        Range,
        Delta
    }

    public class DeviceBinding : INotifyPropertyChanged
    {
        /* Persistence */
        private bool _isBound;
        public bool IsBound
        {
            get => _isBound;
            set
            {
                _isBound = value;
                OnPropertyChanged();
            }
        }
        // Index in its device list
        public Guid DeviceGuid { get; set; }
        // Subscription key
        public int KeyType { get; set; }
        public int KeyValue { get; set; }
        public int KeySubValue { get; set; }

        /* Runtime */
        [XmlIgnore]
        public Guid Guid { get; }
        [XmlIgnore]
        public Profile Profile { get; set; }
        [XmlIgnore]
        public DeviceIoType DeviceIoType { get; set; }
        [XmlIgnore]
        public DeviceBindingCategory DeviceBindingCategory { get; set; }

        private bool _isInBindMode = false;
        [XmlIgnore]
        public bool IsInBindMode
        {
            get => _isInBindMode;
            private set
            {
                _isInBindMode = value;
                OnPropertyChanged();
            }
        }


        public delegate void ValueChanged(short value);
        
        private Action<short> _callback;

        [XmlIgnore]
        public Action<short> Callback
        {
            get => InputChanged;
            set
            {
                _callback = value;
                OnPropertyChanged();
            }
        }
        [XmlIgnore]
        public ValueChanged OutputSink { get; set; }

        private short _currentValue;
        [XmlIgnore]
        public short CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChanged();
            }
        }

        public DeviceBinding()
        {
            Guid = Guid.NewGuid();
        }

        public DeviceBinding(Action<short> callback, Profile profile, DeviceIoType deviceIoType)
        {
            Callback = callback;
            Profile = profile;
            DeviceIoType = deviceIoType;
            Guid = Guid.NewGuid();
            IsBound = false;
        }

        public DeviceBinding(DeviceBinding deviceBinding)
        {
            DeviceGuid = deviceBinding.DeviceGuid;
            KeyType = deviceBinding.KeyType;
            KeyValue = deviceBinding.KeyValue;
            Profile = deviceBinding.Profile;
            Callback = deviceBinding.Callback;
            Guid = deviceBinding.Guid;
            IsBound = deviceBinding.IsBound;
        }

        public void SetDeviceGuid(Guid deviceGuid)
        {
            DeviceGuid = deviceGuid;
            Profile.Context.ContextChanged();
        }

        public void SetKeyTypeValue(int type, int value, int subValue)
        {
            KeyType = type;
            KeyValue = value;
            KeySubValue = subValue;
            IsBound = true;
            Profile.Context.ContextChanged();
        }
        
        public string BoundName()
        {
            return Profile.GetDevice(DeviceIoType, DeviceGuid)?.GetBindingName(this) ?? "Device unavailable";
        }

        public static DeviceBindingCategory MapCategory(BindingCategory bindingInfoCategory)
        {
            switch (bindingInfoCategory)
            {
                case BindingCategory.Event:
                    return DeviceBindingCategory.Event;
                case BindingCategory.Momentary:
                    return DeviceBindingCategory.Momentary;
                case BindingCategory.Signed:
                case BindingCategory.Unsigned:
                    return DeviceBindingCategory.Range;
                case BindingCategory.Delta:
                    return DeviceBindingCategory.Delta;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bindingInfoCategory), bindingInfoCategory, null);
            }
        }

        public void WriteOutput(short value)
        {
            CurrentValue = value;
            OutputSink?.Invoke(value);
        }

        public void EnterBindMode()
        {
            Profile.Context.BindingManager.BeginBindMode(this);
            Profile.Context.BindingManager.EndBindModeHandler += OnEndBindModeHandler;
            IsInBindMode = true;
        }

        private void OnEndBindModeHandler(DeviceBinding deviceBinding)
        {
            if (deviceBinding.Guid != Guid) return;
            IsInBindMode = false;
            Profile.Context.BindingManager.EndBindModeHandler -= OnEndBindModeHandler;
        }

        private void InputChanged(short value)
        {
            CurrentValue = value;
            _callback(value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
