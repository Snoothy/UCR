using System.Collections.Generic;
using HidWizards.UCR.Core.Models.Device;

namespace HidWizards.UCR.Tests.Factory
{
    internal static class DeviceFactory
    {
        public static Device CreateDevice(string title, string providerName, string deviceNumber)
        {
            return new Device()
            {
                Title = title,
                ProviderName = providerName,
                DeviceHandle = deviceNumber
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
