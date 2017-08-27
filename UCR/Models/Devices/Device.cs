using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Providers;
using UCR.Models.Mapping;
using BindingInfo = Providers.BindingInfo;

namespace UCR.Models.Devices
{
    public enum DeviceType
    {
        [Description("Joystick")]
        Joystick,
        [Description("Keyboard")]
        Keyboard,
        [Description("Mouse")]
        Mouse,
        [Description("Generic device")]
        Generic
    }

    public abstract class Device
    {
        // Persistance
        public string Title { get; set; }
        public string DeviceHandle { get; set; }
        public string SubscriberProviderName { get; set; }
        public DeviceType DeviceType { get; }

        // Runtime
        public Guid Guid { get; }
        public bool IsAcquired { get; set; }
        public List<BindingInfo> Bindings { get; set; }

        // Subscriptions
        private Dictionary<string, List<DeviceBinding>> Subscriptions;
        
        protected Device(DeviceType deviceType, Guid guid = new Guid())
        {
            DeviceType = deviceType;
            Guid = (guid == Guid.Empty) ? Guid.NewGuid() : guid;
            IsAcquired = false;
        }

        protected Device(Device device)
        {
            Title = device.Title;
            DeviceType = device.DeviceType;
            DeviceHandle = device.DeviceHandle;
            SubscriberProviderName = device.SubscriberProviderName;
            Bindings = device.Bindings;
            Guid = device.Guid;
        }

        public void WriteOutput(UCRContext ctx, DeviceBinding deviceBinding, long value)
        {
            if (DeviceHandle == null || SubscriberProviderName == null) return;
            ctx.IOController.SetOutputstate(new OutputSubscriptionRequest()
            {
                ProviderName = SubscriberProviderName,
                DeviceHandle = DeviceHandle,
                SubscriberGuid = Guid
            }, (InputType)deviceBinding.KeyType, (uint)deviceBinding.KeyValue, (int)value);
        }

        public bool AddDeviceBinding(DeviceBinding deviceBinding)
        {
            List<DeviceBinding> currentSub = null;
            if (Subscriptions.ContainsKey(deviceBinding.Plugin.Title))
            {
                currentSub = Subscriptions[deviceBinding.Plugin.Title];
            }

            if (currentSub == null || currentSub.Count == 0)
            {
                Subscriptions[deviceBinding.Plugin.Title] = new List<DeviceBinding> {deviceBinding};
                return true;
            }
            else
            {
                // Override bindings if Profile parent does not match. Root is loaded first and active profile last
                if (!string.Equals(currentSub[0].Plugin.ParentProfile.Title, deviceBinding.Plugin.ParentProfile.Title))
                {
                    Subscriptions[deviceBinding.Plugin.Title] = new List<DeviceBinding> {deviceBinding};
                }
                else
                {
                    var existingBinding = currentSub.Find(b => b.Guid == deviceBinding.Guid);
                    if (existingBinding != null)
                    {
                        // Remove existing binding if it exists
                        Subscriptions[deviceBinding.Plugin.Title].Remove(existingBinding);
                    }
                    Subscriptions[deviceBinding.Plugin.Title].Add(deviceBinding);
                }
            }
             
            return true;
        }

        protected void ClearSubscribers()
        {
            Subscriptions = new Dictionary<string, List<DeviceBinding>>();
        }

        public void SubscribeDeviceBindings(UCRContext ctx)
        {
            foreach (var deviceBindingList in Subscriptions)
            {
                foreach (var deviceBinding in deviceBindingList.Value)
                {
                    SubscribeDeviceBindingInput(ctx, deviceBinding);
                }
            }
        }

        public void SubscribeDeviceBindingInput(UCRContext ctx, DeviceBinding deviceBinding)
        {
            if (!deviceBinding.IsBound) return; // TODO unsubscribe binding
            var success = ctx.IOController.SubscribeInput(new InputSubscriptionRequest()
            {
                ProviderName = SubscriberProviderName,
                DeviceHandle = DeviceHandle,
                InputType = (InputType)deviceBinding.KeyType,
                InputIndex = (uint)deviceBinding.KeyValue,
                InputSubId = deviceBinding.KeySubValue,
                Callback = deviceBinding.Callback,
                SubscriberGuid = deviceBinding.Guid,
                ProfileGuid = deviceBinding.Plugin.ParentProfile.Guid
            });
        }

        public string GetBindingName(DeviceBinding deviceBinding)
        {
            if (!deviceBinding.IsBound) return "Not bound";
            return GetBindingName(deviceBinding, Bindings) ?? "Unknown input";
        }

        private static string GetBindingName(DeviceBinding deviceBinding, List<BindingInfo> bindingInfos)
        {
            foreach (var bindingInfo in bindingInfos)
            {
                if (bindingInfo.IsBinding && (int)bindingInfo.InputType == deviceBinding.KeyType && bindingInfo.InputIndex == deviceBinding.KeyValue)
                {
                    return bindingInfo.Title;
                }
                var name = GetBindingName(deviceBinding, bindingInfo.SubBindings);
                if (name != null)
                {
                    return bindingInfo.Title + ", " + name;
                }
            }
            return null;
        }

        public bool SubscribeOutput(UCRContext ctx)
        {
            if (string.IsNullOrEmpty(SubscriberProviderName) || string.IsNullOrEmpty(DeviceHandle))
            {
                // TODO Log error
                return false;
            }
            if (IsAcquired) return true;
            IsAcquired = true;
            return ctx.IOController.SubscribeOutput(new OutputSubscriptionRequest()
            {
                DeviceHandle = DeviceHandle,
                ProviderName = SubscriberProviderName,
                SubscriberGuid = Guid
            });
        }

        public static List<T> CopyDeviceList<T>(List<T> devicelist) where T : new()
        {
            List<T> newDevicelist = new List<T>();
            if (devicelist == null) return newDevicelist;

            foreach (var device in devicelist)
            {
                newDevicelist.Add((T)Activator.CreateInstance(typeof(T), device));
            }

            return newDevicelist;
        }

        public static List<KeyValuePair<int, string>> ZipValuesWithName(List<int> values, Dictionary<int, string> names)
        {
            var result = new List<KeyValuePair<int,string>>();
            foreach (var value in values)
            {
                var name = (names?[value] != null) ? names[value] : value.ToString();
                result.Add(new KeyValuePair<int, string>(value, name));
            }
            return result;
        }
    }

}
