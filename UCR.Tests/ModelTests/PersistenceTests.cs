using NUnit.Framework;
using UCR.Core;
using UCR.Core.Device;
using UCR.Core.Plugins;
using UCR.Tests.Factory;

namespace UCR.Tests.ModelTests
{
    [TestFixture]
    internal class PersistenceTests
    {
        private readonly int _saveReloadTimes = 3;

        [Test]
        public void BlankContext()
        {
            var ctx = new UCRContext();
            ctx.SaveContext();

            for (var i = 0; i < _saveReloadTimes; i++)
            {
                var newCtx = UCRContext.Load();
                Assert.That(newCtx.IsNotSaved, Is.False);
                Assert.That(newCtx.ActiveProfile, Is.Null);
                Assert.That(newCtx.Profiles, Is.Not.Null.And.Empty);
                Assert.That(newCtx.JoystickGroups, Is.Not.Null.And.Empty);
                Assert.That(newCtx.KeyboardGroups, Is.Not.Null.And.Empty);
                Assert.That(newCtx.MiceGroups, Is.Not.Null.And.Empty);
                Assert.That(newCtx.GenericDeviceGroups, Is.Not.Null.And.Empty);
                newCtx.SaveContext();
            }
        }

        [Test]
        public void ProfileContext()
        {
            var ctx = new UCRContext();
            ctx.AddProfile("Root profile");
            ctx.Profiles[0].AddNewChildProfile("Child profile");
            ctx.SaveContext();

            for (var i = 0; i < _saveReloadTimes; i++)
            {
                var newCtx = UCRContext.Load();
                Assert.That(newCtx.Profiles.Count, Is.EqualTo(1));
                Assert.That(newCtx.Profiles[0].ChildProfiles.Count, Is.EqualTo(1));
                Assert.That(newCtx.Profiles[0], Is.EqualTo(newCtx.Profiles[0].ChildProfiles[0].ParentProfile));

                Assert.That(newCtx.Profiles[0].Title, Is.EqualTo(ctx.Profiles[0].Title));
                Assert.That(newCtx.Profiles[0].Guid, Is.EqualTo(ctx.Profiles[0].Guid));
                Assert.That(newCtx.Profiles[0].Plugins.Count, Is.EqualTo(ctx.Profiles[0].Plugins.Count));

                Assert.That(newCtx.Profiles[0].ChildProfiles[0].Title, Is.EqualTo(ctx.Profiles[0].ChildProfiles[0].Title));
                Assert.That(newCtx.Profiles[0].ChildProfiles[0].Guid, Is.EqualTo(ctx.Profiles[0].ChildProfiles[0].Guid));
                Assert.That(newCtx.Profiles[0].ChildProfiles[0].Plugins.Count, Is.EqualTo(ctx.Profiles[0].ChildProfiles[0].Plugins.Count));
                newCtx.SaveContext();
            }
        }

        [Test]
        public void PluginContext()
        {
            var ctx = new UCRContext();
            ctx.AddProfile("Root profile");

            var profile = ctx.Profiles[0];
            profile.AddPlugin(new ButtonToButton(), "Button to button 1");
            profile.AddPlugin(new ButtonToButton(), "Button to button 2");

            for (var i = 0; i < profile.Plugins.Count; i++)
            {
                SetDeviceBindingValues(profile.Plugins[i].Inputs[0], i + 1);
                SetDeviceBindingValues(profile.Plugins[i].Outputs[0], i + 2);
            }

            var plugins = profile.Plugins;
            ctx.SaveContext();

            for (var i = 0; i < _saveReloadTimes; i++)
            {
                var newCtx = UCRContext.Load();
                var newProfile = newCtx.Profiles[0];
                Assert.That(newProfile.Plugins.Count, Is.EqualTo(profile.Plugins.Count));

                for (var j = 0; j < plugins.Count; j++)
                {
                    Assert.That(newProfile.Plugins[j].Title, Is.EqualTo(plugins[j].Title));
                    Assert.That(newProfile.Plugins[j].Inputs.Count, Is.EqualTo(plugins[j].Inputs.Count));
                    Assert.That(newProfile.Plugins[j].Outputs.Count, Is.EqualTo(plugins[j].Outputs.Count));
                    Assert.That(newProfile.Plugins[j].Inputs[0].Guid, Is.Not.EqualTo(plugins[j].Inputs[0].Guid));
                    Assert.That(newProfile.Plugins[j].Inputs[0].IsBound, Is.EqualTo(plugins[j].Inputs[0].IsBound));
                    Assert.That(newProfile.Plugins[j].Inputs[0].DeviceNumber, Is.EqualTo(plugins[j].Inputs[0].DeviceNumber));
                    Assert.That(newProfile.Plugins[j].Inputs[0].KeyType, Is.EqualTo(plugins[j].Inputs[0].KeyType));
                    Assert.That(newProfile.Plugins[j].Inputs[0].KeyValue, Is.EqualTo(plugins[j].Inputs[0].KeyValue));
                    Assert.That(newProfile.Plugins[j].Inputs[0].KeySubValue, Is.EqualTo(plugins[j].Inputs[0].KeySubValue));
                }

                Assert.That(((ButtonToButton)newProfile.Plugins[0]).Input.Guid, Is.EqualTo(newProfile.Plugins[0].Inputs[0].Guid));
                Assert.That(((ButtonToButton)newProfile.Plugins[0]).Output.Guid, Is.EqualTo(newProfile.Plugins[0].Outputs[0].Guid));

                newCtx.SaveContext();
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
            var ctx = new UCRContext();
            var joystickGuid = ctx.AddDeviceGroup("Joystick 1", DeviceType.Joystick);
            ctx.AddDeviceGroup("Joystick 2", DeviceType.Joystick);
            ctx.AddDeviceGroup("Joystick 3", DeviceType.Joystick);
            ctx.AddDeviceGroup("Joystick 4", DeviceType.Joystick);
            var keyboardGuid = ctx.AddDeviceGroup("Keyboard", DeviceType.Keyboard);
            ctx.AddDeviceGroup("Mice", DeviceType.Mouse);
            ctx.AddDeviceGroup("Generic", DeviceType.Generic);

            var joystickDeviceGroup = ctx.GetDeviceGroup(DeviceType.Joystick, joystickGuid);
            joystickDeviceGroup.Devices = DeviceFactory.CreateDeviceList("Gamepad", "Gamepad provider", 4);
            var keyboardDeviceGroup = ctx.GetDeviceGroup(DeviceType.Keyboard, keyboardGuid);
            keyboardDeviceGroup.Devices = DeviceFactory.CreateDeviceList("Keyboard", "interception", 1);

            ctx.SaveContext();

            for (var i = 0; i < _saveReloadTimes; i++)
            {
                var newCtx = UCRContext.Load();
                
                Assert.That(newCtx.JoystickGroups.Count, Is.EqualTo(ctx.JoystickGroups.Count));
                Assert.That(newCtx.KeyboardGroups.Count, Is.EqualTo(ctx.KeyboardGroups.Count));
                Assert.That(newCtx.MiceGroups.Count, Is.EqualTo(ctx.MiceGroups.Count));
                Assert.That(newCtx.GenericDeviceGroups.Count, Is.EqualTo(ctx.GenericDeviceGroups.Count));

                for (var j = 0; j < ctx.JoystickGroups.Count; j++)
                {
                    Assert.That(newCtx.JoystickGroups[j].Guid, Is.EqualTo(ctx.JoystickGroups[j].Guid));
                    Assert.That(newCtx.JoystickGroups[j].Title, Is.EqualTo(ctx.JoystickGroups[j].Title));

                    for (var k = 0; k < ctx.JoystickGroups[j].Devices.Count; k++)
                    {
                        Assert.That(newCtx.JoystickGroups[j].Devices[k].Title, Is.EqualTo(ctx.JoystickGroups[j].Devices[k].Title));
                        Assert.That(newCtx.JoystickGroups[j].Devices[k].DeviceHandle, Is.EqualTo(ctx.JoystickGroups[j].Devices[k].DeviceHandle));
                        Assert.That(newCtx.JoystickGroups[j].Devices[k].ProviderName, Is.EqualTo(ctx.JoystickGroups[j].Devices[k].ProviderName));
                        Assert.That(newCtx.JoystickGroups[j].Devices[k].SubProviderName, Is.EqualTo(ctx.JoystickGroups[j].Devices[k].SubProviderName));
                    }
                }

                newCtx.SaveContext();
            }
        }
    }
}
