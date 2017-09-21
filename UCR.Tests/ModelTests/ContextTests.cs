using System;
using NUnit.Framework;
using UCR.Core;

namespace UCR.Tests.ModelTests
{
    [TestFixture]
    public class ContextTests
    {
        [Test]
        public void InitializationTest()
        {
            var context = new Context();
            Assert.That(context, Is.Not.Null);
            Assert.That(context.Profiles, Is.Not.Null);
            Assert.That(context.JoystickGroups, Is.Not.Null);
            Assert.That(context.KeyboardGroups, Is.Not.Null);
            Assert.That(context.MiceGroups, Is.Not.Null);
            Assert.That(context.GenericDeviceGroups, Is.Not.Null);
            Assert.That(context.IsNotSaved, Is.False);
        }

        [Test]
        public void AddProfile()
        {
            var context = new Context();
            Assert.That(context.Profiles.Count, Is.EqualTo(0));
            var profileName = "Test";
            context.ProfilesController.AddProfile(profileName);
            Assert.That(context.Profiles.Count, Is.EqualTo(1));
            Assert.That(context.Profiles[0].Title, Is.EqualTo(profileName));
            Assert.That(context.Profiles[0].Guid, Is.Not.EqualTo(Guid.Empty));
        }
    }
}
