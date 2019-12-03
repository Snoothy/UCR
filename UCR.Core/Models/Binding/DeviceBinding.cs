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
        [XmlAttribute]
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
        [XmlAttribute]
        public Guid DeviceConfigurationGuid { get; set; }
        // Subscription key
        [XmlAttribute]
        public int KeyType { get; set; }
        [XmlAttribute]
        public int KeyValue { get; set; }
        [XmlAttribute]
        public int KeySubValue { get; set; }
        [XmlAttribute]
        [DefaultValue(false)]
        public bool Block { get; set; }

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

        public void SetDeviceConfigurationGuid(Guid deviceConfigurationGuid)
        {
            DeviceConfigurationGuid = deviceConfigurationGuid;
            if (Block && !IsBlockable()) Block = false;
            Profile.Context.ContextChanged();
            OnPropertyChanged(nameof(DeviceConfigurationGuid));
        }

        public void SetBlock(bool block)
        {
            Block = block;
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
            return Profile.GetDeviceConfiguration(DeviceIoType, DeviceConfigurationGuid)?.Device.GetBindingName(this) ?? "Device unavailable";
        }

        public bool IsBlockable()
        {
            var device = Profile.GetDeviceConfiguration(DeviceIoType, DeviceConfigurationGuid)?.Device;
            if (device == null) return false;

            var deviceBindingNodes = Profile.Context.DevicesManager.GetDeviceBindingMenu(device, DeviceIoType);

            var searchList = deviceBindingNodes;

            while (searchList.Count > 0)
            {
                var node = searchList[0];
                searchList.RemoveAt(0);

                if (node.IsBinding)
                {
                    var info = node.DeviceBindingInfo;
                    if (info.KeyType == KeyType && info.KeyValue == KeyValue && info.KeySubValue == KeySubValue) return info.Blockable;
                }

                if (node.ChildrenNodes != null) searchList.AddRange(node.ChildrenNodes);
            }

            return false;
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

        public void ClearBinding()
        {
            KeyType = 0;
            KeyValue = 0;
            KeySubValue = 0;
            DeviceConfigurationGuid = Guid.Empty;
            IsBound = false;
            Profile.Context.ContextChanged();
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
