using System;
using HidWizards.UCR.Tests.Factory;
using NUnit.Framework;

namespace HidWizards.UCR.Tests.FactoryTests
{
    [TestFixture]
    class DeviceFactoryTests
    {
        [Test]
        public void CreateDevice()
        {
            var title = "Test device";
            var providerName = "Test provider";
            var deviceNumber = 0;
            var device = DeviceFactory.CreateDevice(title, providerName, deviceNumber.ToString(), deviceNumber);
            Assert.That(device.Title, Is.EqualTo(title));
            Assert.That(device.ProviderName, Is.EqualTo(providerName));
            Assert.That(device.DeviceHandle, Is.EqualTo(deviceNumber.ToString()));
        }

        [Test]
        public void CreateDeviceList()
        {
            var title = "Test device";
            var providerName = "Test provider";
            var deviceList = DeviceFactory.CreateDeviceList(title, providerName, 4);
            Assert.That(deviceList.Count, Is.EqualTo(4));
            Assert.That(deviceList[0].Title, Is.Not.EqualTo(deviceList[1].Title));
            Assert.That(deviceList[0].ProviderName, Is.EqualTo(deviceList[1].ProviderName));
            Assert.That(deviceList[0].DeviceHandle, Is.EqualTo(0.ToString()));
            Assert.That(deviceList[3].DeviceHandle, Is.EqualTo(3.ToString()));
        }
    }
}
