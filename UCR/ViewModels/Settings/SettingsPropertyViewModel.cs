using System.Windows;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Settings;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class SettingsPropertyViewModel
    {
        public string Title => SettingsProperty.Title;
        public string Description => SettingsProperty.Description;
        public Visibility SeparatorVisibility => LastElement ? Visibility.Collapsed : Visibility.Visible;

        public dynamic Property
        {
            get => SettingsProperty.Property;
            set => SettingsProperty.Property = value;
        }

        public SettingsProperty SettingsProperty { get; set; }

        public bool LastElement { get; set; }

        public SettingsPropertyViewModel(SettingsProperty settingsProperty)
        {
            SettingsProperty = settingsProperty;
        }
    }
}