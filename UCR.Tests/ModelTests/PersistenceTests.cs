using System;
using System.Collections.Generic;
using System.Diagnostics;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Plugins.Remapper;
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
            context.SaveContext(null);

            for (var i = 0; i < _saveReloadTimes; i++)
            {
                var newcontext = Context.Load();
                Assert.That(newcontext.IsNotSaved, Is.False);
                Assert.That(newcontext.ActiveProfile, Is.Null);
                Assert.That(newcontext.Profiles, Is.Not.Null.And.Empty);
                newcontext.SaveContext(null);
            }
        }

        [Test]
        public void ProfileContext()
        {
            var context = new Context();
            var profile = context.ProfilesManager.CreateProfile("Root Profile", null, null);
            var childProfile = context.ProfilesManager.CreateProfile("Child Profile", null, null);
            context.ProfilesManager.AddProfile(profile);
            context.Profiles[0].AddChildProfile(childProfile);
            context.SaveContext(null);

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
                context.SaveContext(null);
            }
        }

        [Test]
        public void MappingContext()
        {
            var context = new Context();
            var pluginTypes = new List<Type>();
            pluginTypes.Add(typeof(ButtonToButton));
            var rootProfile = context.ProfilesManager.CreateProfile("Root Profile", null, null);
            context.ProfilesManager.AddProfile(rootProfile);
            var profile = context.Profiles[0];
            var mapping = profile.AddMapping("Jump");
            profile.AddPlugin(mapping, new ButtonToButton());
            profile.AddPlugin(mapping, new ButtonToButton());

            var bindingCount = profile.Mappings[0].DeviceBindings.Count;
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
                Assert.That(newMapping.Plugins[0].Outputs.Count, Is.EqualTo(1));

                for (var j = 0; j < mapping.DeviceBindings.Count; j++)
                {
                    Assert.That(newDeviceBindings[j].Profile.Guid, Is.Not.EqualTo(Guid.Empty));
                    Assert.That(newDeviceBindings[j].Profile.Guid, Is.EqualTo(deviceBindings[j].Profile.Guid));
                    Assert.That(newDeviceBindings[j].DeviceIoType, Is.EqualTo(DeviceIoType.Input));
                    Assert.That(newDeviceBindings[j].Guid, Is.Not.EqualTo(deviceBindings[j].Guid));
                    Assert.That(newDeviceBindings[j].IsBound, Is.EqualTo(deviceBindings[j].IsBound));
                    Assert.That(newDeviceBindings[j].DeviceConfigurationGuid, Is.EqualTo(deviceBindings[j].DeviceConfigurationGuid));
                    Assert.That(newDeviceBindings[j].KeyType, Is.EqualTo(deviceBindings[j].KeyType));
                    Assert.That(newDeviceBindings[j].KeyValue, Is.EqualTo(deviceBindings[j].KeyValue));
                    Assert.That(newDeviceBindings[j].KeySubValue, Is.EqualTo(deviceBindings[j].KeySubValue));
                }

                newcontext.SaveContext(pluginTypes);
            }
        }

        private static void SetDeviceBindingValues(DeviceBinding deviceBinding, int value)
        {
            deviceBinding.DeviceConfigurationGuid = Guid.NewGuid();
            deviceBinding.KeyType = value;
            deviceBinding.KeyValue = value;
            deviceBinding.KeySubValue = value;
        }

    }
}
