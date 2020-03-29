using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Models.Settings;
using SettingsProperty = HidWizards.UCR.Core.Models.Settings.SettingsProperty;

namespace HidWizards.UCR.Core.Managers
{
    public class SettingsManager
    {
        public List<SettingsProperty> SettingsProperties { get; }

        public SettingsManager()
        {
            SettingsProperties = GenerateSettingsList();
        }

        private List<SettingsProperty> GenerateSettingsList()
        {
            var settingsProperties = new List<SettingsProperty>();

            foreach (var propertyInfo in typeof(SettingsCollection).GetProperties())
            {
                var settingsProperty = new SettingsProperty(propertyInfo);
                settingsProperty.PropertyChanged += SettingChanged;
                settingsProperties.Add(settingsProperty);
            }

            return settingsProperties;
        }

        private void SettingChanged(object sender, PropertyChangedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
