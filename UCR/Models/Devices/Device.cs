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

    public class Device
    {
        // Persistance
        public string Title { get; set; }
        public string SubscriberProviderName { get; set; }
        public string DeviceHandle { get; set; }
        public string SubscriberSubProviderName { get; set; }

        // Runtime
        public Guid Guid { get; }
        public bool IsAcquired { get; set; }
        public List<BindingInfo> Bindings { get; set; }
        public string Api { get; }

        // Subscriptions
        private Dictionary<string, List<DeviceBinding>> Subscriptions;

        public Device(Guid guid = new Guid())
        {
            Guid = (guid == Guid.Empty) ? Guid.NewGuid() : guid;
            IsAcquired = false;
            ClearSubscribers();
        }

        public Device(Device device) : this(device.Guid)
        {
            Title = device.Title;
            DeviceHandle = device.DeviceHandle;
            SubscriberProviderName = device.SubscriberProviderName;
            SubscriberSubProviderName = device.SubscriberSubProviderName;
            Bindings = device.Bindings;
            Guid = device.Guid;
        }

        public Device(IOWrapperDevice device) : this()
        {
            Title = device.DeviceName;
            DeviceHandle = device.DeviceHandle;
            SubscriberProviderName = device.ProviderName;
            SubscriberSubProviderName = device.SubProviderName;
            Bindings = device.Bindings;
            Api = device.API;
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

        private void ClearSubscribers()
        {
            Subscriptions = new Dictionary<string, List<DeviceBinding>>();
        }

        public bool SubscribeDeviceBindings(UCRContext ctx)
        {
            var success = true;
            foreach (var deviceBindingList in Subscriptions)
            {
                foreach (var deviceBinding in deviceBindingList.Value)
                {
                    success &= SubscribeDeviceBindingInput(ctx, deviceBinding);
                }
            }
            return success;
        }

        public bool UnsubscribeDeviceBindings(UCRContext ctx)
        {
            var success = true;
            foreach (var deviceBindingList in Subscriptions)
            {
                foreach (var deviceBinding in deviceBindingList.Value)
                {
                    success &= UnsubscribeDeviceBindingInput(ctx, deviceBinding);
                }
            }
            return success;
        }

        /// <summary>
        /// Subscribe a devicebining with the backend. Unsubscribe devicebinding if it is not bound
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="deviceBinding"></param>
        /// <returns>If the subscription succeeded</returns>
        public bool SubscribeDeviceBindingInput(UCRContext ctx, DeviceBinding deviceBinding)
        {
            return deviceBinding.IsBound 
                ? ctx.IOController.SubscribeInput(GetInputSubscriptionRequest(deviceBinding))
                : UnsubscribeDeviceBindingInput(ctx, deviceBinding);
        }

        public bool UnsubscribeDeviceBindingInput(UCRContext ctx, DeviceBinding deviceBinding)
        {
            return ctx.IOController.UnsubscribeInput(GetInputSubscriptionRequest(deviceBinding));
        }

        private InputSubscriptionRequest GetInputSubscriptionRequest(DeviceBinding deviceBinding)
        {
            return new InputSubscriptionRequest()
            {
                ProviderName = SubscriberProviderName,
                SubProviderName = SubscriberSubProviderName,
                DeviceHandle = DeviceHandle,
                InputType = (InputType) deviceBinding.KeyType,
                InputIndex = (uint) deviceBinding.KeyValue,
                InputSubId = deviceBinding.KeySubValue,
                Callback = deviceBinding.Callback,
                SubscriberGuid = deviceBinding.Guid,
                ProfileGuid = deviceBinding.Plugin.ParentProfile.Guid
            };
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
            return ctx.IOController.SubscribeOutput(GetOutputSubscriptionRequest());
        }

        public bool UnsubscribeOutput(UCRContext ctx)
        {
            if (string.IsNullOrEmpty(SubscriberProviderName) || string.IsNullOrEmpty(DeviceHandle))
            {
                // TODO Log error
                return false;
            }
            if (!IsAcquired) return true;
            IsAcquired = false;

            return false;
            //return ctx.IOController.UnsubscribeOutput(GetOutputSubscriptionRequest());
        }

        private OutputSubscriptionRequest GetOutputSubscriptionRequest()
        {
            return new OutputSubscriptionRequest()
            {
                DeviceHandle = DeviceHandle,
                ProviderName = SubscriberProviderName,
                SubProviderName = SubscriberSubProviderName,
                SubscriberGuid = Guid
            };
        }

        public static List<Device> CopyDeviceList(List<Device> devicelist)
        {
            var newDevicelist = new List<Device>();
            if (devicelist == null) return newDevicelist;

            newDevicelist.AddRange(devicelist);
            return newDevicelist;
        }
    }
}
