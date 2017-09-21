using System.Collections.Generic;

namespace UCR.Core.Controllers
{
    public class ProfilesController
    {
        private readonly Context _context;
        private readonly List<Profile.Profile> _profiles;

        public ProfilesController(Context context, List<Profile.Profile> profiles)
        {
            _context = context;
            _profiles = profiles;
        }

        public bool AddProfile(string title)
        {
            _profiles.Add(Profile.Profile.CreateProfile(_context, title));
            _context.ContextChanged();
            return true;
        }

        public bool ActivateProfile(Profile.Profile profile)
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

        public bool DeactivateProfile(Profile.Profile profile)
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
