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

        /* Persistence */
        [XmlAttribute]
        public string Title { get; set; }
        [XmlAttribute]
        public string ProviderName { get; set; }
        [XmlAttribute]
        public string DeviceHandle { get; set; }
        [XmlAttribute]
        public int DeviceNumber { get; set; }

        /* Runtime */
        [XmlIgnore]
        private List<DeviceBindingNode> DeviceBindingMenu { get; set; }
        [XmlIgnore] public Profile Profile { get; set; }
        [XmlIgnore] public bool IsCache { get; set; }

        #region Constructors

        public Device()
        {
        }

        public Device(string title, string providerName, string deviceHandle, int deviceNumber)
        {
            Title = title;
            ProviderName = providerName;
            DeviceHandle = deviceHandle;
            DeviceNumber = deviceNumber;
        }

        public Device(DeviceReport device, ProviderReport providerReport, List<DeviceBindingNode> deviceBindingMenu) : this()
        {
            Title = device.DeviceName;
            ProviderName = providerReport.ProviderDescriptor.ProviderName;
            DeviceHandle = device.DeviceDescriptor.DeviceHandle;
            DeviceNumber = device.DeviceDescriptor.DeviceInstance;
            DeviceBindingMenu = deviceBindingMenu;
            IsCache = false;
        }

        public Device(DeviceCache deviceCache)
        {
            Title = deviceCache.Title;
            ProviderName = deviceCache.ProviderName;
            DeviceHandle = deviceCache.DeviceHandle;
            DeviceNumber = deviceCache.DeviceNumber;
            DeviceBindingMenu = deviceCache.DeviceBindingMenu;
            IsCache = true;
        }

        #endregion
        
        public string GetBindingName(DeviceBinding deviceBinding)
        {
            if (!deviceBinding.IsBound) return "Not bound";
            return GetBindingName(deviceBinding, GetDeviceBindingMenu(deviceBinding.Profile.Context, deviceBinding.DeviceIoType)) ?? "Unknown input";
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
                   deviceBindingNode.DeviceBindingInfo.KeyType == deviceBinding.KeyType &&
                   deviceBindingNode.DeviceBindingInfo.KeySubValue == deviceBinding.KeySubValue &&
                   deviceBindingNode.DeviceBindingInfo.KeyValue == deviceBinding.KeyValue;
        }

        public List<DeviceBindingNode> GetDeviceBindingMenu()
        {
            if (DeviceBindingMenu != null && DeviceBindingMenu.Count != 0) return DeviceBindingMenu;

            return new List<DeviceBindingNode>
            {
                new DeviceBindingNode()
                {
                    Title = "Device not connected",
                }
            };
        }

        public List<DeviceBindingNode> GetDeviceBindingMenu(Context context, DeviceIoType type)
        {
            if (DeviceBindingMenu != null && DeviceBindingMenu.Count != 0) return DeviceBindingMenu;

            return context.DevicesManager.GetDeviceBindingMenu(this, type);
        }

        public string LogName()
        {
            return $"Device:{{{Title}}} Provider:{{{ProviderName}}} Handle:{{{DeviceHandle}}} Num:{{{DeviceNumber}}}";
        }

        public override bool Equals(Object other)
        {
            if ((other == null) || GetType() != other.GetType()) return false;
            var otherDevice = other as Device;
            return string.Equals(ProviderName, otherDevice.ProviderName) && string.Equals(DeviceHandle, otherDevice.DeviceHandle) && DeviceNumber == otherDevice.DeviceNumber;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ProviderName != null ? ProviderName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DeviceHandle != null ? DeviceHandle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ DeviceNumber;
                return hashCode;
            }
        }
    }
}
