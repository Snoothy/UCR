using System;
using NUnit.Framework;
using UCR.Core;

namespace UCR.Tests.ModelTests
{
    [TestFixture]
    public class UCRContextTests
    {
        [Test]
        public void InitializationTest()
        {
            var ctx = new UCRContext();
            Assert.That(ctx, Is.Not.Null);
            Assert.That(ctx.Profiles, Is.Not.Null);
            Assert.That(ctx.JoystickGroups, Is.Not.Null);
            Assert.That(ctx.KeyboardGroups, Is.Not.Null);
            Assert.That(ctx.MiceGroups, Is.Not.Null);
            Assert.That(ctx.GenericDeviceGroups, Is.Not.Null);
            Assert.That(ctx.IsNotSaved, Is.False);
        }

        [Test]
        public void AddProfile()
        {
            var ctx = new UCRContext();
            Assert.That(ctx.Profiles.Count, Is.EqualTo(0));
            var profileName = "Test";
            ctx.AddProfile(profileName);
            Assert.That(ctx.Profiles.Count, Is.EqualTo(1));
            Assert.That(ctx.Profiles[0].Title, Is.EqualTo(profileName));
            Assert.That(ctx.Profiles[0].Guid, Is.Not.EqualTo(Guid.Empty));
        }
    }
}
