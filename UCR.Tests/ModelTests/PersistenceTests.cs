using System;
using System.Collections.Generic;
using System.Diagnostics;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Plugins.AxisToAxis;
using HidWizards.UCR.Plugins.ButtonToButton;
using HidWizards.UCR.Tests.Factory;
using NUnit.Framework;

namespace HidWizards.UCR.Tests.ModelTests
{
    [TestFixture]
    internal class PersistenceTests
    {
        private readonly int _saveReloadTimes = 3;

        [Test]
        public void BlankContext()
        {
            var context = new Context();
            context.SaveContext();

            for (var i = 0; i < _saveReloadTimes; i++)
            {
                var newcontext = Context.Load();
                Assert.That(newcontext.IsNotSaved, Is.False);
                Assert.That(newcontext.ActiveProfile, Is.Null);
                Assert.That(newcontext.Profiles, Is.Not.Null.And.Empty);
                Assert.That(newcontext.InputGroups, Is.Not.Null.And.Empty);
                Assert.That(newcontext.OutputGroups, Is.Not.Null.And.Empty);
                newcontext.SaveContext();
            }
        }

        [Test]
        public void ProfileContext()
        {
            var context = new Context();
            context.ProfilesManager.AddProfile("Root profile");
            context.Profiles[0].AddNewChildProfile("Child profile");
            context.SaveContext();

            Assert.That(context.Profiles.Count, Is.EqualTo(1));
            Assert.That(context.Profiles[0].ChildProfiles.Count, Is.EqualTo(1));

            for (var i = 0; i < _saveReloadTimes; i++)
            {
                var newcontext = Context.Load();
                Assert.That(newcontext.Profiles.Count, Is.EqualTo(1));
                Assert.That(newcontext.Profiles[0].ChildProfiles.Count, Is.EqualTo(1));
                Assert.That(newcontext.Profiles[0], Is.EqualTo(newcontext.Profiles[0].ChildProfiles[0].ParentProfile));

                Assert.That(newcontext.Profiles[0].Title, Is.EqualTo(context.Profiles[0].Title));
                Assert.That(newcontext.Profiles[0].Guid, Is.EqualTo(context.Profiles[0].Guid));
                Assert.That(newcontext.Profiles[0].Mappings.Count, Is.EqualTo(context.Profiles[0].Mappings.Count));

                Assert.That(newcontext.Profiles[0].ChildProfiles[0].Title,
                    Is.EqualTo(context.Profiles[0].ChildProfiles[0].Title));
                Assert.That(newcontext.Profiles[0].ChildProfiles[0].Guid,
                    Is.EqualTo(context.Profiles[0].ChildProfiles[0].Guid));
                Assert.That(newcontext.Profiles[0].ChildProfiles[0].Mappings.Count,
                    Is.EqualTo(context.Profiles[0].ChildProfiles[0].Mappings.Count));
                newcontext.SaveContext();
            }
        }

        [Test]
        public void MappingContext()
        {
            var context = new Context();
            var pluginTypes = new List<Type>();
            pluginTypes.Add(typeof(ButtonToButton));
            context.ProfilesManager.AddProfile("Root profile");
            var profile = context.Profiles[0];
            var mapping = profile.AddMapping("Jump");
            profile.AddPlugin(mapping, new ButtonToButton(), Guid.Empty);
            profile.AddPlugin(mapping, new ButtonToButton(), Guid.Empty);

            var bindingCount = 10;
            mapping.InitializeMappings(bindingCount);
            for (var i = 0; i < bindingCount; i++)
            {
                SetDeviceBindingValues(profile.Mappings[0].DeviceBindings[i], i + 1);
            }

            var deviceBindings = mapping.DeviceBindings;
            context.SaveContext(pluginTypes);

            for (var i = 0; i < _saveReloadTimes; i++)
            {
                var newcontext = Context.Load(pluginTypes);
                var newMapping = newcontext.Profiles[0].Mappings[0];
                var newDeviceBindings = newcontext.Profiles[0].Mappings[0].DeviceBindings;
                Assert.That(newMapping.Title, Is.EqualTo(mapping.Title));
                Assert.That(newMapping.Plugins.Count, Is.EqualTo(mapping.Plugins.Count));
                Assert.That(newMapping.Guid, Is.EqualTo(mapping.Guid));
                Assert.That(newMapping.Plugins[0].Outputs.Count, Is.EqualTo(1));

                for (var j = 0; j < mapping.DeviceBindings.Count; j++)
                {
                    Assert.That(newDeviceBindings[j].Profile.Guid, Is.Not.EqualTo(Guid.Empty));
                    Assert.That(newDeviceBindings[j].Profile.Guid, Is.EqualTo(deviceBindings[j].Profile.Guid));
                    Assert.That(newDeviceBindings[j].DeviceIoType, Is.EqualTo(DeviceIoType.Input));
                    Assert.That(newDeviceBindings[j].Guid, Is.Not.EqualTo(deviceBindings[j].Guid));
                    Assert.That(newDeviceBindings[j].IsBound, Is.EqualTo(deviceBindings[j].IsBound));
                    Assert.That(newDeviceBindings[j].DeviceGuid, Is.EqualTo(deviceBindings[j].DeviceGuid));
                    Assert.That(newDeviceBindings[j].KeyType, Is.EqualTo(deviceBindings[j].KeyType));
                    Assert.That(newDeviceBindings[j].KeyValue, Is.EqualTo(deviceBindings[j].KeyValue));
                    Assert.That(newDeviceBindings[j].KeySubValue, Is.EqualTo(deviceBindings[j].KeySubValue));
                }

                newcontext.SaveContext(pluginTypes);
            }
        }

        private static void SetDeviceBindingValues(DeviceBinding deviceBinding, int value)
        {
            deviceBinding.DeviceGuid = Guid.NewGuid();
            deviceBinding.KeyType = value;
            deviceBinding.KeyValue = value;
            deviceBinding.KeySubValue = value;
        }

        [Test]
        public void DeviceListContext()
        {
            var context = new Context();
            var joystickGuid = context.DeviceGroupsManager.AddDeviceGroup("Joystick 1", DeviceIoType.Input);
            context.DeviceGroupsManager.AddDeviceGroup("Joystick 2", DeviceIoType.Input);
            context.DeviceGroupsManager.AddDeviceGroup("Joystick 3", DeviceIoType.Input);
            context.DeviceGroupsManager.AddDeviceGroup("Joystick 4", DeviceIoType.Input);
            var keyboardGuid = context.DeviceGroupsManager.AddDeviceGroup("Keyboard", DeviceIoType.Output);

            var joystickDeviceGroup = context.DeviceGroupsManager.GetDeviceGroup(DeviceIoType.Input, joystickGuid);
            joystickDeviceGroup.Devices = DeviceFactory.CreateDeviceList("Gamepad", "Gamepad provider", 4);
            var keyboardDeviceGroup = context.DeviceGroupsManager.GetDeviceGroup(DeviceIoType.Output, keyboardGuid);
            keyboardDeviceGroup.Devices = DeviceFactory.CreateDeviceList("Keyboard", "interception", 1);

            context.SaveContext();

            for (var i = 0; i < _saveReloadTimes; i++)
            {
                var newcontext = Context.Load();

                Assert.That(newcontext.InputGroups.Count, Is.EqualTo(context.InputGroups.Count));
                Assert.That(newcontext.OutputGroups.Count, Is.EqualTo(context.OutputGroups.Count));

                for (var j = 0; j < context.InputGroups.Count; j++)
                {
                    Assert.That(newcontext.InputGroups[j].Guid, Is.EqualTo(context.InputGroups[j].Guid));
                    Assert.That(newcontext.InputGroups[j].Title, Is.EqualTo(context.InputGroups[j].Title));

                    for (var k = 0; k < context.InputGroups[j].Devices.Count; k++)
                    {
                        Assert.That(newcontext.InputGroups[j].Devices[k].Title,
                            Is.EqualTo(context.InputGroups[j].Devices[k].Title));
                        Assert.That(newcontext.InputGroups[j].Devices[k].DeviceHandle,
                            Is.EqualTo(context.InputGroups[j].Devices[k].DeviceHandle));
                        Assert.That(newcontext.InputGroups[j].Devices[k].ProviderName,
                            Is.EqualTo(context.InputGroups[j].Devices[k].ProviderName));
                    }
                }

                newcontext.SaveContext();
            }
        }
    }
}
