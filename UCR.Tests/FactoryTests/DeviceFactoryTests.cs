using System;
using NUnit.Framework;
using UCR.Tests.Factory;

namespace UCR.Tests.FactoryTests
{
    [TestFixture]
    class DeviceFactoryTests
    {
        [Test]
        public void CreateDevice()
        {
            var title = "Test device";
            var providerName = "Test provider";
            var deviceNumber = 0.ToString();
            var device = DeviceFactory.CreateDevice(title, providerName, deviceNumber);
            Assert.That(device.Guid, Is.Not.EqualTo(Guid.Empty));
            Assert.That(device.Title, Is.EqualTo(title));
            Assert.That(device.SubscriberProviderName, Is.EqualTo(providerName));
            Assert.That(device.SubscriberSubProviderName, Is.Not.Null);
            Assert.That(device.SubscriberSubProviderName, Is.Not.EqualTo(device.SubscriberProviderName));
            Assert.That(device.DeviceHandle, Is.EqualTo(deviceNumber));
        }

        [Test]
        public void CreateDeviceList()
        {
            var title = "Test device";
            var providerName = "Test provider";
            var deviceList = DeviceFactory.CreateDeviceList(title, providerName, 4);
            Assert.That(deviceList.Count, Is.EqualTo(4));
            Assert.That(deviceList[0].Title, Is.Not.EqualTo(deviceList[1].Title));
            Assert.That(deviceList[0].SubscriberProviderName, Is.EqualTo(deviceList[1].SubscriberProviderName));
            Assert.That(deviceList[0].DeviceHandle, Is.EqualTo(0.ToString()));
            Assert.That(deviceList[3].DeviceHandle, Is.EqualTo(3.ToString()));
        }
    }
}
