using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class ProfileItem
    {
        public ProfileItem()
        {
            Items = new ObservableCollection<ProfileItem>();
        }

        public string Title { get; set; }
        public Guid Id { get; set; }
        public Profile Profile { get; set; }

        public ObservableCollection<ProfileItem> Items { get; set; }

        public static ObservableCollection<ProfileItem> GetProfileTree(List<Profile> profiles)
        {
            var profileItems = new ObservableCollection<ProfileItem>();
            if (profiles == null) return profileItems;

            foreach (var profile in profiles)
            {
                profileItems.Add(new ProfileItem()
                {
                    Title = profile.Title,
                    Id = profile.Guid,
                    Items = GetProfileTree(profile.ChildProfiles),
                    Profile = profile
                });
            }
            
            return profileItems;
        }

    }
}
