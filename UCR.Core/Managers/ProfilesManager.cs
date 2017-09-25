using System;
using System.Collections.Generic;
using System.Reflection;
using UCR.Core.Models.Profile;

namespace UCR.Core.Managers
{
    public class ProfilesManager
    {
        private readonly Context _context;
        private readonly List<Profile> _profiles;

        public ProfilesManager(Context context, List<Profile> profiles)
        {
            _context = context;
            _profiles = profiles;
        }

        public bool AddProfile(string title)
        {
            _profiles.Add(Profile.CreateProfile(_context, title));
            _context.ContextChanged();
            return true;
        }

        public bool ActivateProfile(Profile profile)
        {
            var success = true;
            if (_context.ActiveProfile?.Guid == profile.Guid) return success;
            var lastActiveProfile = _context.ActiveProfile;
            _context.ActiveProfile = profile;
            success &= profile.Activate(_context);
            if (success)
            {
                var subscribeSuccess = profile.SubscribeDeviceLists();
                _context.IOController.SetProfileState(profile.Guid, true);
                DeactivateProfile(lastActiveProfile);
                foreach (var action in _context.ActiveProfileCallbacks)
                {
                    action();
                }
            }
            else
            {
                // Activation failed, old profile is still active
                _context.ActiveProfile = lastActiveProfile;
            }
            return success;
        }

        public bool CopyProfile(Profile profile, string title = "Untitled")
        {
            var newProfile = Context.DeepXmlClone<Profile>(profile);
            newProfile.Title = title;
            newProfile.Guid = Guid.NewGuid();
            newProfile.PostLoad(_context, profile.ParentProfile);

            if (profile.ParentProfile != null)
            {
                profile.ParentProfile.AddChildProfile(newProfile, title);
            }
            else
            {
                _profiles.Add(newProfile);
            }
            _context.ContextChanged();

            return true;
        }

        public bool DeactivateProfile(Profile profile)
        {
            if (_context.ActiveProfile == null || profile == null) return true;
            if (_context.ActiveProfile.Guid == profile.Guid) _context.ActiveProfile = null;

            var success = profile.UnsubscribeDeviceLists();
            _context.IOController.SetProfileState(profile.Guid, false);

            foreach (var action in _context.ActiveProfileCallbacks)
            {
                action();
            }
            return success;
        }
    }
}
