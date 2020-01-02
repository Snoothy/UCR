using System;
using System.Collections.Generic;
using System.Linq;
using HidWizards.UCR.Core.Models;
using NLog;

namespace HidWizards.UCR.Core.Managers
{
    public class ProfilesManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Context _context;
        private readonly List<Profile> _profiles;

        public ProfilesManager(Context context, List<Profile> profiles)
        {
            _context = context;
            _profiles = profiles;
        }

        public Profile CreateProfile(string title, List<DeviceConfiguration> inputDevices, List<DeviceConfiguration> outputDevices)
        {
            return Profile.CreateProfile(_context, title, inputDevices, outputDevices);
        }

        public bool AddProfile(Profile newProfile, Profile parentProfile = null)
        {
            if (parentProfile != null)
            {
                parentProfile.AddChildProfile(newProfile);
            }
            else
            {
                _profiles.Add(newProfile);
            }

            _context.ContextChanged();
            return true;
        }

        public bool CopyProfile(Profile profile, string title = "Untitled")
        {
            var newProfile = Context.DeepXmlClone<Profile>(profile);
            newProfile.Title = title;
            newProfile.Guid = Guid.NewGuid();
            newProfile.PostLoad(_context, profile.ParentProfile);

            if (profile.ParentProfile != null)
            {
                profile.ParentProfile.AddChildProfile(newProfile);
            }
            else
            {
                _profiles.Add(newProfile);
            }

            // TODO Fix Configuration Guid and referenced DeviceBinding Guids
            //newProfile.InputDeviceConfigurations.ForEach(configuration => configuration.Guid = Guid.NewGuid());
            //newProfile.OutputDeviceConfigurations.ForEach(configuration => configuration.Guid = Guid.NewGuid());

            _context.ContextChanged();

            return true;
        }

        /// <summary>
        /// Breadth-first search for nested profiles
        /// Find first search result and looks for the next result in the children
        /// </summary>
        /// <param name="search">List of profiles to search for nested under each other</param>
        /// <returns>The most specific profile found in the chain, otherwise null</returns>
        public Profile FindProfile(List<string> search)
        {
            Logger.Debug($"Searching for profile: {{{string.Join(",", search)}}}");
            Profile foundProfile = null;
            if (search?.Count == 0) return null;
            var queue = new List<Profile>();
            queue.AddRange(_profiles);
            while (queue.Count > 0)
            {
                var profile = queue[0];
                queue.RemoveAt(0);
                if (profile.Title.ToLower().Equals(search.First().ToLower()))
                {
                    if (search.Count == 1)
                    {
                        Logger.Debug($"Found profile: {{{profile.ProfileBreadCrumbs()}}}");
                        return profile;
                    }
                    foundProfile = profile;
                    search.RemoveAt(0);
                    Logger.Trace($"Found intermediate profile: {{{profile.ProfileBreadCrumbs()}}}. Remaining search: {{{string.Join(",", search)}}}");
                    queue.Clear();
                }
                if (profile.ChildProfiles != null) queue.AddRange(profile.ChildProfiles);

            }
            if (foundProfile == null) Logger.Debug($"No profile found for {{{string.Join(",", search)}}}");
            return foundProfile;
        }
    }
}
