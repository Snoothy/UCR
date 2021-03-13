using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Shell;

namespace HidWizards.UCR.Views
{
    public static class JumpList
    {
        private static System.Windows.Shell.JumpList jumpList, oldJumpList;
        private static Context _context;

        public static void InitJumpList(Context context)
        {
            _context = context;
            _context.ActiveProfileChangedEvent += ActiveProfileChangedHandler;
            _context.ContextChangedEvent += UpdateJumpList;

            jumpList = new System.Windows.Shell.JumpList();
            oldJumpList = new System.Windows.Shell.JumpList();
            foreach(Guid guid in _context.RecentProfiles)
            {
                Profile profile = _context.ProfilesManager.FindProfile(guid);
                jumpList.JumpItems.Add(new JumpTask
                {
                    Arguments = $"-p {profile.Title}",
                    Title = profile.Title,
                    CustomCategory = "Recent Profiles",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                });
            }
            System.Windows.Shell.JumpList.SetJumpList(Application.Current, jumpList);
        }

        private static void ActiveProfileChangedHandler(Profile profile)
        {
            UpdateJumpList();
        }

        private static void UpdateJumpList()
        {
            if (!_context.IsNotSaved) oldJumpList.JumpItems.Clear(); 
            jumpList.JumpItems.Clear();
            foreach (Guid guid in _context.RecentProfiles)
            {
                Profile _profile = _context.ProfilesManager.FindProfile(guid);
                jumpList.JumpItems.Add(new JumpTask
                {
                    Arguments = $"-p {_profile.Title}",
                    Title = _profile.Title,
                    CustomCategory = "Recent Profiles",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                });
                if (!_context.IsNotSaved)
                {
                    oldJumpList.JumpItems.Add(new JumpTask
                    {
                        Arguments = $"-p {_profile.Title}",
                        Title = _profile.Title,
                        CustomCategory = "Recent Profiles",
                        WorkingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                    });
                }
            }

            try
            {
                jumpList.Apply();
            }
            catch (InvalidOperationException e)
            {
                // Unsupported through CLI
            }
        }

        public static void RestoreJumpList()
        {
            System.Windows.Shell.JumpList.SetJumpList(App.Current, oldJumpList);
        }
    }
}