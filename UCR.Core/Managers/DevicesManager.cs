using System;
using System.Collections.Generic;
using System.Linq;
using HidWizards.IOWrapper.DataTransferObjects;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Managers
{
    public class DevicesManager
    {
        private readonly Context _context;

        public DevicesManager(Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a list of available devices from the backend
        /// </summary>
        /// <param name="type"></param>
        public List<DeviceGroup> GetAvailableDeviceList(DeviceIoType type)
        {
            _context.IOController.RefreshDevices();
            var deviceGroupList = new List<DeviceGroup>();
            var providerList = type == DeviceIoType.Input
                ? _context.IOController.GetInputList()
                : _context.IOController.GetOutputList();

            foreach (var providerReport in providerList)
            {
                var deviceGroup = new DeviceGroup(providerReport.Key);
                foreach (var ioWrapperDevice in providerReport.Value.Devices)
                {
                    deviceGroup.Devices.Add(new Device(ioWrapperDevice, providerReport.Value, BuildDeviceBindingMenu(ioWrapperDevice.Nodes, type)));
                }
                deviceGroupList.Add(deviceGroup);
            }
            return deviceGroupList;
        }

        public List<Device> GetAvailableDevicesListFromSameProvider(DeviceIoType type, Device device)
        {
            var availableDeviceList = GetAvailableDeviceList(type);
            return availableDeviceList.SelectMany(deviceGroup => deviceGroup.Devices).Where(d => d.ProviderName.Equals(device.ProviderName)).ToList();
        }


        public List<DeviceBindingNode> GetDeviceBindingMenu(Device device, DeviceIoType type)
        {
            
            var ioController = _context.IOController;
            var list = type == DeviceIoType.Input
                ? ioController.GetInputList()
                : ioController.GetOutputList();

            try
            {
                // TODO Read cache
                return BuildDeviceBindingMenu(list[device.ProviderName]?.Devices.Find(d => d.DeviceDescriptor.DeviceHandle == device.DeviceHandle)?.Nodes, type);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentNullException)
            {
                return new List<DeviceBindingNode>
                {
                    new DeviceBindingNode()
                    {
                        Title = "Device not connected",
                    }
                };
            }
        }

        private static List<DeviceBindingNode> BuildDeviceBindingMenu(List<DeviceReportNode> deviceNodes, DeviceIoType type)
        {
            var result = new List<DeviceBindingNode>();
            if (deviceNodes == null) return result;

            foreach (var deviceNode in deviceNodes)
            {
                var groupNode = new DeviceBindingNode()
                {
                    Title = deviceNode.Title,
                    ChildrenNodes = BuildDeviceBindingMenu(deviceNode.Nodes, type),
                };

                if (groupNode.ChildrenNodes == null) groupNode.ChildrenNodes = new List<DeviceBindingNode>();
                

                foreach (var bindingInfo in deviceNode.Bindings)
                {
                    var bindingNode = new DeviceBindingNode()
                    {
                        Title = bindingInfo.Title,
                        DeviceBindingInfo = new DeviceBindingInfo()
                        {
                            KeyType = (int)bindingInfo.BindingDescriptor.Type,
                            KeyValue = bindingInfo.BindingDescriptor.Index,
                            KeySubValue = bindingInfo.BindingDescriptor.SubIndex,
                            DeviceBindingCategory = DeviceBinding.MapCategory(bindingInfo.Category)
                        }
                    };


                    groupNode.ChildrenNodes.Add(bindingNode);
                }
                result.Add(groupNode);
            }
            return result.Count != 0 ? result : null;
        }
    }
}