using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Plugins.Remapper;
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
            var profile = _context.ProfilesManager.CreateProfile("Base Profile", null, null);
            _context.ProfilesManager.AddProfile(profile);
            _profile = _context.Profiles[0];
            _mapping = _profile.AddMapping("Test mapping");
            _profileName = "Test";
        }

        [Test]
        public void AddChildProfile()
        {
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(0));
            var childProfile = _context.ProfilesManager.CreateProfile(_profileName, null, null);
            _profile.AddChildProfile(childProfile);
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
            var childProfile = _context.ProfilesManager.CreateProfile(_profileName, null, null);
            _profile.AddChildProfile(childProfile);
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(1));
            Assert.That(_profile.ChildProfiles[0].Title, Is.EqualTo(_profileName));
            _profile.ChildProfiles[0].Remove();
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(0));
            Assert.That(_context.IsNotSaved, Is.True);
        }

        [Test]
        public void RenameProfile()
        {
            var newName = "Renamed Profile";
            Assert.That(_profile.Rename(newName), Is.True);
            Assert.That(_profile.Title, Is.EqualTo(newName));
            Assert.That(_context.IsNotSaved, Is.True);
        }

        [Test]
        public void AddPlugin()
        {
            _profile.AddPlugin(_mapping, new ButtonToButton());
            var plugin = _mapping.Plugins[0];

            Assert.That(plugin, Is.Not.Null);
            Assert.That(plugin.Outputs, Is.Not.Null);
            Assert.That(plugin.Profile, Is.EqualTo(_profile));
            Assert.That(_context.IsNotSaved, Is.True);
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
            var childProfile = _context.ProfilesManager.CreateProfile("Child", null, null);
            parentProfile.AddChildProfile(childProfile);
            var profile = parentProfile.ChildProfiles[0];
            profileManager.CopyProfile(profile, "Copy");
            var newProfile = parentProfile.ChildProfiles[1];

            Assert.That(newProfile.Guid, Is.Not.EqualTo(profile.Guid));
            Assert.That(newProfile.Title, Is.EqualTo("Copy"));
            Assert.That(newProfile.ParentProfile.Guid, Is.EqualTo(parentProfile.Guid));
            Assert.That(newProfile.Context, Is.Not.Null);
        }
    }
}
