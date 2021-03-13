using System.Collections.Generic;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.Tests.Factory
{
    internal static class DeviceFactory
    {
        public static Device CreateDevice(string title, string providerName, string deviceHandle, int deviceNumber)
        {
            return new Device(title, providerName, deviceHandle, deviceNumber);
        }

        public static List<Device> CreateDeviceList(string title, string providerName, int amount)
        {
            var result = new List<Device>();
            if (amount < 1) return result;
            for (var i = 0; i < amount; i++)
            {
                result.Add(CreateDevice(title+i, providerName, i.ToString(), i));
            }
            return result;
        }
    }
}
