using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Attributes;

namespace HidWizards.UCR.Core.Models.Settings
{
    public static class SettingsCollection
    {
        private static Properties.Settings DefaultSettings => Properties.Settings.Default;

        [Setting(Title = "Minimize on close", Description = "Minimize UCR on close instead of exiting")]
        public static bool MinimizeOnClose {
            get => DefaultSettings.MinimizeOnclose;
            set => DefaultSettings.MinimizeOnclose = value;
        }

        [Setting(Title = "Start UCR minimized", Description = "Launches UCR as minimized to tray")]
        public static bool LaunchMinimized
        {
            get => DefaultSettings.LaunchMinimized;
            set => DefaultSettings.LaunchMinimized = value;
        }
    }
}
