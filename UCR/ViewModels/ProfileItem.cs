using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models;

namespace UCR.ViewModels
{
    public class ProfileItem
    {
        public ProfileItem()
        {
            Items = new ObservableCollection<ProfileItem>();
        }

        public string Title { get; set; }
        public long Id { get; set; }
        public Profile profile { get; set; }

        public ObservableCollection<ProfileItem> Items { get; set; }

        public static ObservableCollection<ProfileItem> GetProfileTree(List<Profile> profiles)
        {
            ObservableCollection<ProfileItem> profileItems = new ObservableCollection<ProfileItem>();
            if (profiles == null) return profileItems;

            foreach (var profile in profiles)
            {
                profileItems.Add(new ProfileItem()
                {
                    Title = profile.Title,
                    Id = profile.Id,
                    Items = GetProfileTree(profile.ChildProfiles),
                    profile = profile
                });
            }
            
            return profileItems;
        }

    }
}
