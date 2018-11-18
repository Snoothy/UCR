using System;
using System.Collections.Generic;
using HidWizards.IOWrapper.DataTransferObjects;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using NLog;
using Logger = NLog.Logger;

namespace HidWizards.UCR.Core.Managers
{
    public class BindingManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Context _context;
        private List<Device> _deviceList;
        private DeviceBinding _deviceBinding;

        public delegate void EndBindModeDelegate(DeviceBinding deviceBinding);
        public event EndBindModeDelegate EndBindModeHandler;

        public BindingManager(Context context)
        {
            _context = context;
            _deviceList = new List<Device>();
            Logger.Debug($"Start bind mode");
        }

        public void BeginBindMode(DeviceBinding deviceBinding)
        {
            if (_deviceList.Count > 0) EndBindMode();
            _deviceBinding = deviceBinding;
            foreach (var device in deviceBinding.Profile.GetDeviceList(deviceBinding))
            {
                
                _context.IOController.SetDetectionMode(DetectionMode.Bind, GetProviderDescriptor(device), GetDeviceDescriptor(device), InputChanged);
                _deviceList.Add(device);
            }    
        }

        private void EndBindMode()
        {
            Logger.Debug($"End bind mode");
            EndBindModeHandler?.Invoke(_deviceBinding);

            foreach (var device in _deviceList)
            {
                _context.IOController.SetDetectionMode(DetectionMode.Subscription, GetProviderDescriptor(device), GetDeviceDescriptor(device));
            }
            _deviceList = new List<Device>();
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

        private void InputChanged(ProviderDescriptor providerDescriptor, DeviceDescriptor deviceDescriptor, BindingReport bindingReport, int value)
        {
            if (!DeviceBinding.MapCategory(bindingReport.Category).Equals(_deviceBinding.DeviceBindingCategory)) return;
            if (!IsInputValid(bindingReport.Category, value)) return;

            var device = FindDevice(providerDescriptor, deviceDescriptor);
            _deviceBinding.SetDeviceGuid(device.Guid);
            _deviceBinding.SetKeyTypeValue((int)bindingReport.BindingDescriptor.Type, bindingReport.BindingDescriptor.Index, bindingReport.BindingDescriptor.SubIndex);
            EndBindMode();
        }

        private bool IsInputValid(BindingCategory bindingCategory, int value)
        {
            switch (DeviceBinding.MapCategory(bindingCategory))
            {
                case DeviceBindingCategory.Delta:
                case DeviceBindingCategory.Event:
                    return true;
                case DeviceBindingCategory.Momentary:
                    return value != 0;
                case DeviceBindingCategory.Range:
                    return Constants.AxisMaxValue * 0.2 < Math.Abs(value);
                default:
                    return false;
            }
        }

        private Device FindDevice(ProviderDescriptor providerDescriptor, DeviceDescriptor deviceDescriptor)
        {
            return _deviceList.Find(d => d.ProviderName == providerDescriptor.ProviderName
                                         && d.DeviceHandle == deviceDescriptor.DeviceHandle
                                         && d.DeviceNumber == deviceDescriptor.DeviceInstance
            );
        }

        public void Dispose()
        {
            EndBindMode();
        }
    }
}
