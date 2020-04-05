using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;
using System.Linq;
using System.Windows;
using System.Windows.Shell;

namespace HidWizards.UCR.Views
{
    public static class JumpList
    {
        private static System.Windows.Shell.JumpList jumpList;

        public static void InitJumpList(Context context)
        {
            context.ActiveProfileChangedEvent += AddRecentProfile;
            Profile.ProfileRenamedEvent += ProfileRenamed;
            Profile.ProfileRemovedEvent += ProfileRemoved;

            jumpList = System.Windows.Shell.JumpList.GetJumpList(Application.Current);
            if (jumpList == null) jumpList = new System.Windows.Shell.JumpList();
            System.Windows.Shell.JumpList.SetJumpList(Application.Current, jumpList);
        }

        public static void AddRecentProfile(Profile profile)
        {
            if (profile == null) return;
            if (!jumpList.JumpItems.Any(p => ((JumpTask)p).Title == profile.Title))
            {
                if (jumpList.JumpItems.Count == 5) jumpList.JumpItems.RemoveAt(4);
                jumpList.JumpItems.Insert(0, new JumpTask
                {
                    Arguments = $"-p {profile.Title}",
                    Title = profile.Title,
                    CustomCategory = "Recent Profiles",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                });
            }
            else
            {
                jumpList.JumpItems.RemoveAt(jumpList.JumpItems.FindIndex(p => ((JumpTask)p).Title == profile.Title));
                jumpList.JumpItems.Insert(0, new JumpTask
                {
                    Arguments = $"-p {profile.Title}",
                    Title = profile.Title,
                    CustomCategory = "Recent Profiles",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                });
            }
            jumpList.Apply();
        }

        public static void ProfileRenamed(string oldTitle, string newTitle)
        {
            int index;
            if((index = jumpList.JumpItems.FindIndex(p => ((JumpTask)p).Title == oldTitle)) != -1)
            {
                ((JumpTask)jumpList.JumpItems[index]).Title = newTitle;
                ((JumpTask)jumpList.JumpItems[index]).Arguments = $"-p {newTitle}";
                jumpList.Apply();
            }
        }

        public static void ProfileRemoved(string title)
        {
            int index;
            if ((index = jumpList.JumpItems.FindIndex(p => ((JumpTask)p).Title == title)) != -1)
            {
                jumpList.JumpItems.RemoveAt(index);
                jumpList.Apply();
            }
        }
    }
}