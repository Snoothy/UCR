using System;
using System.Collections.Generic;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Models.Device;
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
                Assert.That(newcontext.Profiles[0].Plugins.Count, Is.EqualTo(context.Profiles[0].Plugins.Count));

                Assert.That(newcontext.Profiles[0].ChildProfiles[0].Title, Is.EqualTo(context.Profiles[0].ChildProfiles[0].Title));
                Assert.That(newcontext.Profiles[0].ChildProfiles[0].Guid, Is.EqualTo(context.Profiles[0].ChildProfiles[0].Guid));
                Assert.That(newcontext.Profiles[0].ChildProfiles[0].Plugins.Count, Is.EqualTo(context.Profiles[0].ChildProfiles[0].Plugins.Count));
                newcontext.SaveContext();
            }
        }

        [Test]
        public void PluginContext()
        {
            var context = new Context();
            var pluginTypes = new List<Type>();
            pluginTypes.Add(typeof(ButtonToButton));
            context.ProfilesManager.AddProfile("Root profile");

            var profile = context.Profiles[0];
            profile.AddPlugin(new ButtonToButton(), "Button to button 1");
            profile.AddPlugin(new ButtonToButton(), "Button to button 2");

            for (var i = 0; i < profile.Plugins.Count; i++)
            {
                SetDeviceBindingValues(profile.Plugins[i].Inputs[0], i + 1);
                SetDeviceBindingValues(profile.Plugins[i].Outputs[0], i + 2);
            }

            var plugins = profile.Plugins;
            context.SaveContext(pluginTypes);

            for (var i = 0; i < _saveReloadTimes; i++)
            {
                var newcontext = Context.Load(pluginTypes);
                var newProfile = newcontext.Profiles[0];
                Assert.That(newProfile.Plugins.Count, Is.EqualTo(profile.Plugins.Count));

                for (var j = 0; j < plugins.Count; j++)
                {
                    Assert.That(newProfile.Plugins[j].Title, Is.EqualTo(plugins[j].Title));
                    Assert.That(newProfile.Plugins[j].Inputs.Count, Is.EqualTo(plugins[j].Inputs.Count));
                    Assert.That(newProfile.Plugins[j].Outputs.Count, Is.EqualTo(plugins[j].Outputs.Count));
                    Assert.That(newProfile.Plugins[j].Outputs[0].DeviceIoType, Is.EqualTo(DeviceIoType.Output));
                    Assert.That(newProfile.Plugins[j].Inputs[0].Guid, Is.Not.EqualTo(plugins[j].Inputs[0].Guid));
                    Assert.That(newProfile.Plugins[j].Inputs[0].IsBound, Is.EqualTo(plugins[j].Inputs[0].IsBound));
                    Assert.That(newProfile.Plugins[j].Inputs[0].DeviceIoType, Is.EqualTo(DeviceIoType.Input));
                    Assert.That(newProfile.Plugins[j].Inputs[0].DeviceNumber, Is.EqualTo(plugins[j].Inputs[0].DeviceNumber));
                    Assert.That(newProfile.Plugins[j].Inputs[0].KeyType, Is.EqualTo(plugins[j].Inputs[0].KeyType));
                    Assert.That(newProfile.Plugins[j].Inputs[0].KeyValue, Is.EqualTo(plugins[j].Inputs[0].KeyValue));
                    Assert.That(newProfile.Plugins[j].Inputs[0].KeySubValue, Is.EqualTo(plugins[j].Inputs[0].KeySubValue));
                }

                Assert.That(((ButtonToButton)newProfile.Plugins[0]).Input.Guid, Is.EqualTo(newProfile.Plugins[0].Inputs[0].Guid));
                Assert.That(((ButtonToButton)newProfile.Plugins[0]).Output.Guid, Is.EqualTo(newProfile.Plugins[0].Outputs[0].Guid));

                newcontext.SaveContext(pluginTypes);
            }
        }

        private static void SetDeviceBindingValues(DeviceBinding deviceBinding, int value)
        {
            deviceBinding.DeviceNumber = value;
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
                        Assert.That(newcontext.InputGroups[j].Devices[k].Title, Is.EqualTo(context.InputGroups[j].Devices[k].Title));
                        Assert.That(newcontext.InputGroups[j].Devices[k].DeviceHandle, Is.EqualTo(context.InputGroups[j].Devices[k].DeviceHandle));
                        Assert.That(newcontext.InputGroups[j].Devices[k].ProviderName, Is.EqualTo(context.InputGroups[j].Devices[k].ProviderName));
                    }
                }

                newcontext.SaveContext();
            }
        }
    }
}
