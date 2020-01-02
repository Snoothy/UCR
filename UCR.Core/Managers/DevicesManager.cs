using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using HidWizards.IOWrapper.DataTransferObjects;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using Newtonsoft.Json;

namespace HidWizards.UCR.Core.Managers
{
    public class DevicesManager
    {
        private readonly Context _context;

        private Dictionary<string, List<Device>> _providerCache;

        public DevicesManager(Context context)
        {
            _context = context;
            _providerCache = new Dictionary<string, List<Device>>();
        }

        /// <summary>
        /// Gets a list of available devices from the backend
        /// </summary>
        /// <param name="type"></param>
        public List<Device> GetAvailableDeviceList(DeviceIoType type, bool includeCache = true)
        {
            var result = new List<Device>();
            var providerList = type == DeviceIoType.Input
                ? _context.IOController.GetInputList()
                : _context.IOController.GetOutputList();

            foreach (var providerReport in providerList)
            {
                foreach (var ioWrapperDevice in providerReport.Value.Devices)
                {
                    result.Add(new Device(ioWrapperDevice, providerReport.Value, BuildDeviceBindingMenu(ioWrapperDevice.Nodes, type)));
                }

                if (includeCache)
                {
                    var cachedDevices = LoadDeviceCache(providerReport.Value.ProviderDescriptor.ProviderName);
                    foreach (var cachedDevice in cachedDevices)
                    {
                        if (result.Contains(cachedDevice)) continue;
                        result.Add(cachedDevice);
                    }
                    
                }
            }
            return result;
        }

        public void RefreshDeviceList()
        {
            _context.IOController.RefreshDevices();
        }

        public List<Device> GetAvailableDevicesListFromSameProvider(DeviceIoType type, Device device)
        {
            var availableDeviceList = GetAvailableDeviceList(type);
            return availableDeviceList.Where(d => d.ProviderName.Equals(device.ProviderName)).ToList();
        }


        public List<DeviceBindingNode> GetDeviceBindingMenu(Device device, DeviceIoType type, bool includeCache = true)
        {
            var availableDeviceList = GetAvailableDeviceList(type, includeCache);

            try
            {
                return availableDeviceList.Find(d => d.DeviceHandle == device.DeviceHandle).GetDeviceBindingMenu();
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ArgumentNullException || ex is NullReferenceException)
            {
                return new List<DeviceBindingNode>
                {
                    new DeviceBindingNode()
                    {
                        Title = "Device not connected"
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
                            DeviceBindingCategory = DeviceBinding.MapCategory(bindingInfo.Category),
                            Blockable = bindingInfo.Blockable
                        }
                    };


                    groupNode.ChildrenNodes.Add(bindingNode);
                }
                result.Add(groupNode);
            }
            return result.Count != 0 ? result : null;
        }

        #region Cache

        public bool UpdateDeviceCache()
        {
            var success = true;
            RefreshDeviceList();
            var availableDeviceList = GetAvailableDeviceList(DeviceIoType.Input, false);
            
            foreach (var device in availableDeviceList)
            {
                success &= SaveDeviceCache(device);
            }

            return success;
        }

        private bool SaveDeviceCache(Device device)
        {
            var serializer = new JsonSerializer();
            Directory.CreateDirectory(GetProviderCacheDirectory(device.ProviderName));
            using (var streamWriter = new StreamWriter(GetDeviceCachePath(device)))
            {
                var deviceCache = new DeviceCache()
                {
                    Title = device.Title,
                    ProviderName = device.ProviderName,
                    DeviceHandle = device.DeviceHandle,
                    DeviceNumber = device.DeviceNumber,
                    DeviceBindingMenu = GetDeviceBindingMenu(device, DeviceIoType.Input, false)
                };

                serializer.Serialize(streamWriter, deviceCache);
            }

            return true;
        }

        private List<Device> LoadDeviceCache(string provider)
        {
            if (_providerCache.ContainsKey(provider)) return _providerCache[provider];

            var result = new List<Device>();
            string[] deviceCacheFiles;
            try
            {
                deviceCacheFiles = Directory.GetFiles(GetProviderCacheDirectory(provider), "*.json",
                    SearchOption.TopDirectoryOnly);
            }
            catch (DirectoryNotFoundException)
            {
                return result;
            }

            foreach (var deviceCacheFile in deviceCacheFiles)
            {
                var device = ReadDeviceCache(provider, deviceCacheFile);
                if (device != null)  result.Add(device);
            }

            _providerCache.Add(provider, result);
            return result;

        }

        private static Device ReadDeviceCache(string provider, string devicePath)
        {
            if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(devicePath)) return null;

            try
            {
                using (var fileStream = new FileStream(devicePath, FileMode.Open))  
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        return new Device(JsonConvert.DeserializeObject<DeviceCache>(reader.ReadToEnd()));
                    }
                }
            }
            catch (IOException e)
            {
                Logger.Error($"Failed to load Cache for Provider: {provider}. Path: {devicePath}", e);
            }
            catch (InvalidOperationException e)
            {
                Logger.Error($"Errors processing XML for Provider cache: {provider}. Path: {devicePath}", e);
            }

            try
            {
                File.Delete(devicePath);
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to delete invalid cache file: {devicePath}", e);
            }

            return null;
        }

        private static string GetDeviceCachePath(Device device)
        {
            return $"{GetProviderCacheDirectory(device.ProviderName)}\\{device.GetHashCode()}.json";
        }

        private static string GetProviderCacheDirectory(string provider)
        {
            return $".\\Cache\\{provider}\\";
        }

        #endregion
    }
}