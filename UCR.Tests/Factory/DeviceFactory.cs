using System;
using System.Collections.Generic;
using UCR.Models.Devices;

namespace UCR.Tests.Factory
{
    internal static class DeviceFactory
    {
        public static Device CreateDevice(string title, string providerName, string deviceNumber)
        {
            return new Device()
            {
                Title = title,
                SubscriberProviderName = providerName,
                DeviceHandle = deviceNumber,
                SubscriberSubProviderName = providerName + "Sub"
            };
        }

        public static List<Device> CreateDeviceList(string title, string providerName, int amount)
        {
            var result = new List<Device>();
            if (amount < 1) return result;
            for (var i = 0; i < amount; i++)
            {
                result.Add(CreateDevice(title+i, providerName, i.ToString()));
            }
            return result;
        }
    }
}
