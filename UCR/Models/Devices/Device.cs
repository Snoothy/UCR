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

        public List<ButtonInfo> SupportedButtons { get; set; }
        public List<AxisInfo> SupportedAxes { get; set; }

        // Subscriptions
        private Dictionary<string, List<DeviceBinding>> Subscriptions;

        // Abstract methods
        protected abstract InputType MapDeviceBindingInputType(DeviceBinding deviceBinding);

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
            SupportedButtons = device.SupportedButtons;
            SupportedAxes = device.SupportedAxes;
            Guid = device.Guid;
        }

        public virtual void WriteOutput(UCRContext ctx, DeviceBinding binding, long value)
        {
            if (DeviceHandle == null || SubscriberProviderName == null) return;
            //SendKeys.SendWait(binding.KeyValue.ToString()); // TODO Keyboard debug
            ctx.IOController.SetOutputstate(new OutputSubscriptionRequest()
            {
                ProviderName = SubscriberProviderName,
                DeviceHandle = DeviceHandle,
                SubscriberGuid = Guid
            }, MapDeviceBindingInputType(binding), (uint)binding.KeyValue, (int)value);
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

        public void ClearSubscribers()
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
            if (!deviceBinding.IsBound) return;
            var success = ctx.IOController.SubscribeInput(new InputSubscriptionRequest()
            {
                InputType = MapDeviceBindingInputType(deviceBinding),
                Callback = deviceBinding.Callback,
                ProviderName = SubscriberProviderName,
                DeviceHandle = DeviceHandle,
                InputIndex = (uint)deviceBinding.KeyValue,
                SubscriberGuid = deviceBinding.Guid,
                ProfileGuid = deviceBinding.Plugin.ParentProfile.Guid
                //InputSubId = TODO
            });
        }

        public virtual string GetBindingName(DeviceBinding deviceBinding)
        {
            // TODO override
            if (deviceBinding.KeyType == (int) KeyType.Axis)
            {
                if (deviceBinding.KeyValue < SupportedAxes.Count)
                {
                    return "Axis " + SupportedAxes[deviceBinding.KeyValue].Name;
                }
                return "Unknown Axis";
            }
            else
            {
                if (deviceBinding.KeyValue < SupportedButtons.Count)
                {
                    return "Button " + SupportedButtons[deviceBinding.KeyValue].Name;
                }
                return "Unknown button";
            }
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
