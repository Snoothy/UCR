using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Settings;
using HidWizards.UCR.ViewModels.ProfileViewModels;

namespace HidWizards.UCR.ViewModels.Settings
{
    public class SettingsViewModel
    {
        public List<SettingsPropertyViewModel> SettingsProperties { get; }

        public SettingsViewModel(SettingsManager settingsManager)
        {
            SettingsProperties = GetPluginPropertyViewModel(settingsManager.SettingsProperties);
        }

        private List<SettingsPropertyViewModel> GetPluginPropertyViewModel(List<SettingsProperty> settingsProperties)
        {
            var settingsPropertyViewModels = new List<SettingsPropertyViewModel>();

            foreach (var settingsProperty in settingsProperties)
            {
                settingsPropertyViewModels.Add(new SettingsPropertyViewModel(settingsProperty));
            }

            if (settingsPropertyViewModels.Count > 0) settingsPropertyViewModels[settingsPropertyViewModels.Count - 1].LastElement = true;
            return settingsPropertyViewModels;
        }
    }
}
