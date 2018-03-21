using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using HidWizards.IOWrapper.DataTransferObjects;
using HidWizards.UCR.Core.Models.Binding;
using NLog;

namespace HidWizards.UCR.Core.Models
{
    public enum DeviceIoType
    {
        Input,
        Output
    }

    public class Device
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        // Persistance
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public string ProviderName { get; set; }
        public string DeviceHandle { get; set; }
        public int DeviceNumber { get; set; }

        // Runtime
        [XmlIgnore]
        private List<DeviceBindingNode> DeviceBindingMenu { get; set; }

        #region Constructors

        public Device()
        {
            Guid = Guid.NewGuid();
        }

        public Device(Guid guid = new Guid())
        {
            Guid = (guid == Guid.Empty) ? Guid.NewGuid() : guid;
        }

        public Device(Device device) : this((Guid) device.Guid)
        {
            Title = device.Title;
            DeviceHandle = device.DeviceHandle;
            ProviderName = device.ProviderName;
            DeviceNumber = device.DeviceNumber;
            DeviceBindingMenu = device.DeviceBindingMenu;
            Guid = device.Guid;
        }

        public Device(DeviceReport device, ProviderReport providerReport, DeviceIoType type) : this()
        {
            Title = device.DeviceName;
            ProviderName = providerReport.ProviderDescriptor.ProviderName;
            DeviceHandle = device.DeviceDescriptor.DeviceHandle;
            DeviceNumber = device.DeviceDescriptor.DeviceInstance;
            DeviceBindingMenu = GetDeviceBindingMenu(device.Nodes, type);
        }

        #endregion

        private static List<DeviceBindingNode> GetDeviceBindingMenu(List<DeviceReportNode> deviceNodes, DeviceIoType type)
        {
            var result = new List<DeviceBindingNode>();
            if (deviceNodes == null) return result;

            foreach (var deviceNode in deviceNodes)
            {
                var groupNode = new DeviceBindingNode()
                {
                    Title = deviceNode.Title,
                    IsBinding = false,
                    ChildrenNodes = GetDeviceBindingMenu(deviceNode.Nodes, type),
                };
                
                foreach (var bindingInfo in deviceNode.Bindings)
                {
                    var bindingNode = new DeviceBindingNode()
                    {
                        Title = bindingInfo.Title,
                        IsBinding = true,
                        DeviceBinding = new DeviceBinding(null,null,type)
                        {
                            IsBound = false,
                            KeyType = (int)bindingInfo.BindingDescriptor.Type,
                            KeyValue = bindingInfo.BindingDescriptor.Index,
                            KeySubValue = bindingInfo.BindingDescriptor.SubIndex,
                            DeviceBindingCategory = DeviceBinding.MapCategory(bindingInfo.Category)
                            // TODO Extract to class instead of using DeviceBinding?
                        }

                    };
                    if (groupNode.ChildrenNodes == null)
                    {
                        groupNode.ChildrenNodes = new List<DeviceBindingNode>();
                    }

                    groupNode.ChildrenNodes.Add(bindingNode);
                }
                result.Add(groupNode);
            }
            return result.Count != 0 ? result : null;
        }

        public string GetBindingName(DeviceBinding deviceBinding)
        {
            if (!deviceBinding.IsBound) return "Not bound";
            return GetBindingName(deviceBinding, DeviceBindingMenu) ?? "Unknown input";
        }

        private static string GetBindingName(DeviceBinding deviceBinding, List<DeviceBindingNode> deviceBindingNodes)
        {
            if (deviceBindingNodes == null) return null;
            foreach (var deviceBindingNode in deviceBindingNodes)
            {
                if (deviceBindingMatchesNode(deviceBinding, deviceBindingNode))
                {
                    return deviceBindingNode.Title;
                }
                var name = GetBindingName(deviceBinding, deviceBindingNode.ChildrenNodes);
                if (name != null)
                {
                    return deviceBindingNode.Title + ", " + name;
                }
            }
            return null;
        }

        private static bool deviceBindingMatchesNode(DeviceBinding deviceBinding, DeviceBindingNode deviceBindingNode)
        {
            return deviceBindingNode.IsBinding && 
                   deviceBindingNode.DeviceBinding.KeyType == deviceBinding.KeyType &&
                   deviceBindingNode.DeviceBinding.KeySubValue == deviceBinding.KeySubValue &&
                   deviceBindingNode.DeviceBinding.KeyValue == deviceBinding.KeyValue;
        }

        public static List<Device> CopyDeviceList(List<Device> devicelist)
        {
            var newDevicelist = new List<Device>();
            if (devicelist == null) return newDevicelist;
            foreach (var device in devicelist)
            {
                newDevicelist.Add(new Device(device));

            }
            return newDevicelist;
        }

        public List<DeviceBindingNode> GetDeviceBindingMenu(Context context, DeviceIoType type)
        {
            if (DeviceBindingMenu == null || DeviceBindingMenu.Count == 0)
            {
                var ioController = context.IOController;
                var list = type == DeviceIoType.Input
                    ? ioController.GetInputList()
                    : ioController.GetOutputList();
                try
                {
                    DeviceBindingMenu = GetDeviceBindingMenu(list[ProviderName]?.Devices.Find(d => d.DeviceDescriptor.DeviceHandle == DeviceHandle)?.Nodes, type);
                }
                catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentNullException)
                {
                    return new List<DeviceBindingNode>
                    {
                        new DeviceBindingNode()
                        {
                            Title = "Device not connected",
                            IsBinding = false
                        }
                    };
                }
            }
            return DeviceBindingMenu;
        }

        public string LogName()
        {
            return $"Device:{{{Title}}} Provider:{{{ProviderName}}} Handle:{{{DeviceHandle}}} Num:{{{DeviceNumber}}}";
        }
    }
}
