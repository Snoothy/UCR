using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using HidWizards.IOWrapper.DataTransferObjects;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using NLog;
using Logger = NLog.Logger;

namespace HidWizards.UCR.Core.Managers
{
    public sealed class BindingManager : IDisposable, INotifyPropertyChanged
    {
        private double _bindModeProgress = 0;

        public double BindModeProgress
        {
            get { return _bindModeProgress / BindModeTime * 100.0; }
            set
            {
                _bindModeProgress = value;
                OnPropertyChanged();
            }
        }

        private static readonly double BindModeTime = 5000.0;
        private static readonly int BindModeTick = 20;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Context _context;
        private List<DeviceConfiguration> _deviceConfigurationList;
        private DeviceBinding _deviceBinding;
        private DispatcherTimer BindingTimer;
        private readonly object bindmodeLock = new object();
        private bool bindmodeActive;

        public delegate void EndBindModeDelegate(DeviceBinding deviceBinding);
        public event EndBindModeDelegate EndBindModeHandler;

        public BindingManager(Context context)
        {
            _context = context;
            _deviceConfigurationList = new List<DeviceConfiguration>();
            Logger.Debug($"Start bind mode");
        }

        public void BeginBindMode(DeviceBinding deviceBinding)
        {
            if (_deviceConfigurationList.Count > 0) EndBindMode();
            _deviceBinding = deviceBinding;
            foreach (var deviceConfiguration in deviceBinding.Profile.GetDeviceConfigurationList(deviceBinding.DeviceIoType))
            {
                _context.IOController.SetDetectionMode(DetectionMode.Bind, GetProviderDescriptor(deviceConfiguration.Device), GetDeviceDescriptor(deviceConfiguration.Device), InputChanged);
                _deviceConfigurationList.Add(deviceConfiguration);
            }

            BindingTimer = new DispatcherTimer(DispatcherPriority.Render);
            BindingTimer.Tick += BindingTimerOnTick;
            BindingTimer.Interval = TimeSpan.FromMilliseconds(BindModeTick);
            BindModeProgress = BindModeTime;
            BindingTimer.Start();
            bindmodeActive = true;
        }

        private void BindingTimerOnTick(object sender, EventArgs e)
        {
            BindModeProgress = _bindModeProgress - BindModeTick;
            if (BindModeProgress <= 0.0) EndBindMode();
        }

        private void EndBindMode()
        {
            lock (bindmodeLock)
            {
                Logger.Debug($"End bind mode");
                if (!bindmodeActive) return;

                EndBindModeHandler?.Invoke(_deviceBinding);
                BindingTimer.Stop();

                foreach (var deviceConfiguration in _deviceConfigurationList)
                {
                    _context.IOController.SetDetectionMode(DetectionMode.Subscription, GetProviderDescriptor(deviceConfiguration.Device),
                        GetDeviceDescriptor(deviceConfiguration.Device));
                }

                _deviceConfigurationList = new List<DeviceConfiguration>();
                BindingTimer.Stop();
                bindmodeActive = false;
            }
        }

        private DeviceDescriptor GetDeviceDescriptor(Device device)
        {
            return new DeviceDescriptor()
            {
                DeviceHandle = device.DeviceHandle,
                DeviceInstance = device.DeviceNumber
            };
        }

        private ProviderDescriptor GetProviderDescriptor(Device device)
        {
            return new ProviderDescriptor()
            {
                ProviderName = device.ProviderName
            };
        }

        private void InputChanged(ProviderDescriptor providerDescriptor, DeviceDescriptor deviceDescriptor, BindingReport bindingReport, short value)
        {
            if (!DeviceBinding.MapCategory(bindingReport.Category).Equals(_deviceBinding.DeviceBindingCategory)) return;
            if (!IsInputValid(bindingReport.Category, value)) return;

            var deviceConfiguration = FindDeviceConfiguration(providerDescriptor, deviceDescriptor);
            _deviceBinding.SetDeviceConfigurationGuid(deviceConfiguration.Guid);
            _deviceBinding.SetKeyTypeValue((int)bindingReport.BindingDescriptor.Type, bindingReport.BindingDescriptor.Index, bindingReport.BindingDescriptor.SubIndex);
            EndBindMode();
        }

        private bool IsInputValid(BindingCategory bindingCategory, short value)
        {
            switch (DeviceBinding.MapCategory(bindingCategory))
            {
                case DeviceBindingCategory.Delta:
                case DeviceBindingCategory.Event:
                    return true;
                case DeviceBindingCategory.Momentary:
                    return value != 0;
                case DeviceBindingCategory.Range:
                    var wideVal = Functions.WideAbs(value);
                    return Constants.AxisMaxValue * 0.4 < wideVal
                        && Constants.AxisMaxValue * 0.6 > wideVal;
                default:
                    return false;
            }
        }

        private DeviceConfiguration FindDeviceConfiguration(ProviderDescriptor providerDescriptor, DeviceDescriptor deviceDescriptor)
        {
            return _deviceConfigurationList.Find(deviceConfiguration => deviceConfiguration.Device.ProviderName == providerDescriptor.ProviderName
                                         && deviceConfiguration.Device.DeviceHandle == deviceDescriptor.DeviceHandle
                                         && deviceConfiguration.Device.DeviceNumber == deviceDescriptor.DeviceInstance
            );
        }

        public void Dispose()
        {
            EndBindMode();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
