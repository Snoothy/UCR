using System;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Plugins.ButtonToButton;
using HidWizards.UCR.Tests.Factory;
using NUnit.Framework;

namespace HidWizards.UCR.Tests.ModelTests
{
    [TestFixture]
    internal class ProfileTests
    {
        private Context _context;
        private Profile _profile;
        private Mapping _mapping;
        private string _profileName;

        [SetUp]
        public void Setup()
        {
            _context = new Context();
            _context.ProfilesManager.AddProfile("Base profile");
            _profile = _context.Profiles[0];
            _mapping = _profile.AddMapping("Test mapping");
            _profileName = "Test";
        }

        [Test]
        public void AddChildProfile()
        {
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(0));
            _profile.AddNewChildProfile(_profileName);
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(1));
            Assert.That(_profile.ChildProfiles[0].Title, Is.EqualTo(_profileName));
            Assert.That(_profile.ChildProfiles[0].ParentProfile, Is.EqualTo(_profile));
            Assert.That(_profile.ChildProfiles[0].Guid, Is.Not.EqualTo(Guid.Empty));
            Assert.That(_profile.IsActive, Is.Not.True);
            Assert.That(_context.IsNotSaved, Is.True);
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
            Assert.That(_context.IsNotSaved, Is.True);
        }

        [Test]
        public void RenameProfile()
        {
            var newName = "Renamed profile";
            Assert.That(_profile.Rename(newName), Is.True);
            Assert.That(_profile.Title, Is.EqualTo(newName));
            Assert.That(_context.IsNotSaved, Is.True);
        }

        [Test]
        public void AddPlugin()
        {
            var pluginState = "State";
            _profile.AddPlugin(_mapping, new ButtonToButton(), pluginState);
            var plugin = _mapping.Plugins[0];
            Assert.That(plugin, Is.Not.Null);
            Assert.That(plugin.State, Is.EqualTo(pluginState));
            Assert.That(plugin.Output, Is.Not.Null);
            Assert.That(plugin.Profile, Is.EqualTo(_profile));
            Assert.That(_context.IsNotSaved, Is.True);
        }

        [Test]
        public void GetDevice()
        {
            var guid = _context.DeviceGroupsManager.AddDeviceGroup("Test joysticks", DeviceIoType.Input);
            var deviceList = DeviceFactory.CreateDeviceList("Dummy", "Provider", 1);
            _context.DeviceGroupsManager.GetDeviceGroup(DeviceIoType.Input, guid).Devices = deviceList;
            var deviceBinding = new DeviceBinding(null, null, DeviceIoType.Input)
            {
                IsBound = true,
                DeviceGuid = deviceList[0].Guid
            };

            Assert.That(_profile.GetDevice(deviceBinding), Is.Null);
            
            Assert.That(guid, Is.Not.EqualTo(Guid.Empty));
            _profile.SetDeviceGroup(deviceBinding.DeviceIoType, guid);
            Assert.That(_context.IsNotSaved, Is.True);
            Assert.That(_profile.GetDevice(deviceBinding), Is.Not.Null);
            Assert.That(_profile.GetDevice(deviceBinding).Guid, Is.EqualTo(_profile.GetDeviceList(deviceBinding)[0].Guid));
        }

        [Test]
        public void GetDeviceList()
        {
            var deviceBinding = new DeviceBinding(null, null, DeviceIoType.Input)
            {
                IsBound = true
            };
            Assert.That(_profile.GetDeviceList(deviceBinding), Is.Empty);
            var guid = _context.DeviceGroupsManager.AddDeviceGroup("Test joysticks", DeviceIoType.Input);
            _profile.SetDeviceGroup(deviceBinding.DeviceIoType, guid);
            Assert.That(_profile.GetDeviceList(deviceBinding), Is.Not.Null.And.Empty);
            _context.DeviceGroupsManager.GetDeviceGroup(DeviceIoType.Input, guid).Devices = DeviceFactory.CreateDeviceList("Dummy", "Provider", 1);
            Assert.That(_profile.GetDeviceList(deviceBinding), Is.Not.Empty);
        }

        [Test, Ignore("Irrelevant")]
        public void GetDeviceFromParent()
        {
            var deviceBinding = new DeviceBinding(null, null, DeviceIoType.Input)
            {
                IsBound = true
            };

            var singleGuid = _context.DeviceGroupsManager.AddDeviceGroup("Single device", DeviceIoType.Input);
            var deviceList = DeviceFactory.CreateDeviceList("Dummy", "Provider", 1);
            _context.DeviceGroupsManager.GetDeviceGroup(deviceBinding.DeviceIoType, singleGuid).Devices = deviceList;
            var multipleGuid = _context.DeviceGroupsManager.AddDeviceGroup("Test joysticks", DeviceIoType.Input);
            _context.DeviceGroupsManager.GetDeviceGroup(deviceBinding.DeviceIoType, multipleGuid).Devices = DeviceFactory.CreateDeviceList("Dummy", "Provider", 4);

            _profile.SetDeviceGroup(deviceBinding.DeviceIoType, multipleGuid);

            _profile.AddNewChildProfile("Child profile");
            var childProfile = _profile.ChildProfiles[0];
            childProfile.SetDeviceGroup(deviceBinding.DeviceIoType, singleGuid);

            Assert.That(childProfile.GetDevice(deviceBinding).Guid, Is.EqualTo(deviceList[0].Guid));

            Assert.That(_profile.GetDevice(deviceBinding).Guid, Is.Not.EqualTo(childProfile.GetDevice(deviceBinding).Guid));
            Assert.That(_profile.GetDevice(deviceBinding).Guid, Is.EqualTo(childProfile.GetDevice(deviceBinding).Guid));
            Assert.That(childProfile.GetDeviceList(deviceBinding).Count, Is.EqualTo(4));
        }

        [Test]
        public void CopyProfile()
        {
            var profileManager = new ProfilesManager(_context, _context.Profiles);
            var profile = _context.Profiles[0];
            profileManager.CopyProfile(profile, "Copy");
            var newProfile = _context.Profiles[1];
            Assert.That(newProfile.Guid, Is.Not.EqualTo(profile.Guid));
            Assert.That(newProfile.Title, Is.EqualTo("Copy"));
            Assert.That(newProfile.ParentProfile, Is.Null);
            Assert.That(newProfile.Context, Is.Not.Null);
        }

        [Test]
        public void CopyChildProfile()
        {
            var profileManager = new ProfilesManager(_context, _context.Profiles);
            var parentProfile = _context.Profiles[0];
            parentProfile.AddNewChildProfile("Child");
            var profile = parentProfile.ChildProfiles[0];
            profileManager.CopyProfile(profile, "Copy");
            var newProfile = parentProfile.ChildProfiles[1];
            Assert.That(newProfile.Guid, Is.Not.EqualTo(profile.Guid));
            Assert.That(newProfile.Title, Is.EqualTo("Copy"));
            Assert.That(newProfile.ParentProfile.Guid, Is.EqualTo(parentProfile.Guid));
            Assert.That(newProfile.Context, Is.Not.Null);
        }

        [Test]
        public void ActivateProfile()
        {
            // TODO
            //_profile.
        }
    }
}
