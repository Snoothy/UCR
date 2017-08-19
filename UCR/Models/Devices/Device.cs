using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Providers;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public enum DeviceType
    {
        [Description("Keyboard")]
        Keyboard,
        [Description("Mouse")]
        Mouse,
        [Description("Joystick")]
        Joystick,
        [Description("Generic device")]
        Generic
    }

    public abstract class Device
    {
        public string Title { get; set; }
        public string Guid { get; set; }
        public string SubscriberProviderName { get; set; }
        public string VID { get; set; }
        public string PID { get; set; }
        public DeviceType DeviceType { get; }

        public abstract bool SubscribeInput(DeviceBinding deviceBinding);
        public abstract bool SubscribeOutput(DeviceBinding deviceBinding);
        public abstract void ClearSubscribers();
        public abstract void SubscribeDeviceBindings(UCRContext ctx);
        public abstract void SubscribeDeviceBindingInput(UCRContext ctx, DeviceBinding deviceBinding);

        public Device()
        {
            
        }

        public Device(DeviceType deviceType)
        {
            this.DeviceType = deviceType;
        }

        public virtual void WriteOutput(UCRContext ctx, DeviceBinding binding, long value)
        {
            if (Guid == null || SubscriberProviderName == null) return;
            ctx.IOController.SetOutputstate(new OutputSubscriptionRequest()
            {
                ProviderName = SubscriberProviderName,
                DeviceHandle = Guid,
                SubscriberGuid = new Guid() // TODO Handle this!
            }, Providers.InputType.BUTTON, (uint)binding.KeyValue, (int)value);
        }

        public bool SubscribeOutput(UCRContext ctx)
        {
            return ctx.IOController.SubscribeOutput(new OutputSubscriptionRequest()
            {
                DeviceHandle = Guid,
                ProviderName = SubscriberProviderName,
                SubscriberGuid = new Guid() // TODO Handle this!
            });
        }

        public Device(Device device)
        {
            Title = device.Title;
            DeviceType = device.DeviceType;
            Guid = device.Guid;
            VID = device.VID;
            PID = device.PID;
            SubscriberProviderName = device.SubscriberProviderName;
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
    }

}
