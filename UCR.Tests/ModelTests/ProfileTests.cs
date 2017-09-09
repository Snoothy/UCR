using System;
using NUnit.Framework;
using UCR.Models;
using UCR.Models.Devices;
using UCR.Models.Mapping;
using UCR.Models.Plugins.Remapper;
using UCR.Tests.Factory;

namespace UCR.Tests.ModelTests
{
    [TestFixture]
    internal class ProfileTests
    {
        private UCRContext _ctx;
        private Profile _profile;
        private string _profileName;

        [SetUp]
        public void Setup()
        {
            _ctx = new UCRContext(false);
            _ctx.AddProfile("Base profile");
            _profile = _ctx.Profiles[0];
            _profileName = "Test";
        }

        [Test]
        public void AddChildProfile()
        {
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(0));
            _profile.AddNewChildProfile(_profileName);
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(1));
            Assert.That(_profile.ChildProfiles[0].Title, Is.EqualTo(_profileName));
            Assert.That(_profile.ChildProfiles[0].Parent, Is.EqualTo(_profile));
            Assert.That(_profile.ChildProfiles[0].Guid, Is.Not.EqualTo(Guid.Empty));
            Assert.That(_profile.IsActive, Is.Not.True);
            Assert.That(_ctx.IsNotSaved, Is.True);
        }
        
        [Test]
        public void RemoveChildProfile()
        {
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(0));
            _profile.AddNewChildProfile(_profileName);
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(1));
            Assert.That(_profile.ChildProfiles[0].Title, Is.EqualTo(_profileName));
            _profile.ChildProfiles[0].Remove();
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(0));
            Assert.That(_ctx.IsNotSaved, Is.True);
        }

        [Test]
        public void RenameProfile()
        {
            var newName = "Renamed profile";
            Assert.That(_profile.Rename(newName), Is.True);
            Assert.That(_profile.Title, Is.EqualTo(newName));
            Assert.That(_ctx.IsNotSaved, Is.True);
        }

        [Test]
        public void AddPlugin()
        {
            var pluginName = "Test plugin";
            _profile.AddPlugin(new ButtonToButton(), pluginName);
            var plugin = _profile.Plugins[0];
            Assert.That(plugin, Is.Not.Null);
            Assert.That(plugin.Title, Is.EqualTo(pluginName));
            Assert.That(plugin.Inputs, Is.Not.Null);
            Assert.That(plugin.Outputs, Is.Not.Null);
            Assert.That(plugin.ParentProfile, Is.EqualTo(_profile));
            Assert.That(_ctx.IsNotSaved, Is.True);
        }

        [Test]
        public void GetDevice()
        {
            var deviceBinding = new DeviceBinding(null, null, DeviceBindingType.Input)
            {
                DeviceType = DeviceType.Joystick,
                DeviceNumber = 0,
                IsBound = true
            };
            Assert.That(_profile.GetDevice(deviceBinding), Is.Null);
            var guid = _ctx.AddDeviceGroup("Test joysticks", DeviceType.Joystick);
            _ctx.GetDeviceGroup(deviceBinding.DeviceType, guid).Devices = DeviceFactory.CreateDeviceList("Dummy", "Provider", 1);
            Assert.That(guid, Is.Not.EqualTo(Guid.Empty));
            _profile.SetDeviceGroup(deviceBinding.DeviceBindingType, deviceBinding.DeviceType, guid);
            Assert.That(_ctx.IsNotSaved, Is.True);
            Assert.That(_profile.GetDevice(deviceBinding), Is.Not.Null);
            Assert.That(_profile.GetDevice(deviceBinding).Guid, Is.EqualTo(_profile.GetDeviceList(deviceBinding)[0].Guid));
        }

        [Test]
        public void GetDeviceList()
        {
            var deviceBinding = new DeviceBinding(null, null, DeviceBindingType.Input)
            {
                DeviceType = DeviceType.Joystick,
                DeviceNumber = 0,
                IsBound = true
            };
            Assert.That(_profile.GetDeviceList(deviceBinding), Is.Empty);
            var guid = _ctx.AddDeviceGroup("Test joysticks", DeviceType.Joystick);
            _profile.SetDeviceGroup(deviceBinding.DeviceBindingType, deviceBinding.DeviceType, guid);
            Assert.That(_profile.GetDeviceList(deviceBinding), Is.Not.Null.And.Empty);
            _ctx.GetDeviceGroup(deviceBinding.DeviceType, guid).Devices = DeviceFactory.CreateDeviceList("Dummy", "Provider", 1);
            Assert.That(_profile.GetDeviceList(deviceBinding), Is.Not.Empty);
        }

        [Test]
        public void GetDeviceFromParent()
        {
            // TODO test that devices are fetched from parent profiles, too
        }

        [Test]
        public void ActivateProfile()
        {
            // TODO
            //_profile.
        }
    }
}
