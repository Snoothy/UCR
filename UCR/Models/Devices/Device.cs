using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public enum DeviceType
    {
        Keyboard,
        Mouse,
        Joystick
    }

    public abstract class Device
    {
        public String Title { get; set; }
        public String Guid { get; set; }
        public String SubscriberPluginName { get; set; }
        public String VID { get; set; }
        public String PID { get; set; }
        public DeviceType DeviceType { get; }

        public abstract bool Subscribe(DeviceBinding deviceBinding);
        public abstract void ClearSubscribers();
        public abstract void Activate(UCRContext ctx);

        public Device()
        {
            
        }

        public Device(DeviceType deviceType)
        {
            this.DeviceType = deviceType;
        }

        public Device(Device device)
        {
            Title = device.Title;
            DeviceType = device.DeviceType;
            Guid = device.Guid;
            VID = device.VID;
            PID = device.PID;
            SubscriberPluginName = device.SubscriberPluginName;
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
