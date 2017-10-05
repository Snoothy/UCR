using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Breadth-first search for nested profiles
        /// Find first search result and looks for the next result in the children
        /// </summary>
        /// <param name="search">List of profiles to search for nested under each other</param>
        /// <returns>The most specific profile found in the chain, otherwise null</returns>
        public Profile FindProfile(List<string> search)
        {
            Profile foundProfile = null;
            if (search?.Count == 0) return null;
            var queue = new List<Profile>();
            queue.AddRange(_profiles);
            while (queue.Count > 0)
            {
                var profile = queue[0];
                queue.RemoveAt(0);
                if (profile.Title.ToLower().Equals(search.First()))
                {
                    if (search.Count == 1) return profile;
                    foundProfile = profile;
                    search.RemoveAt(0);
                    queue.Clear();
                }
                if (profile.ChildProfiles != null) queue.AddRange(profile.ChildProfiles);

            }
            return foundProfile;
        }
    }
}
