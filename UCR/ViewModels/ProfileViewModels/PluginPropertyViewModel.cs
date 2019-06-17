using System.Windows;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginPropertyViewModel
    {
        public string Name => PluginProperty.Name;

        public dynamic Property
        {
            get { return PluginProperty.Property; }
            set { PluginProperty.Property = value; }
        }

        public Visibility SeparatorVisibility => LastElement ? Visibility.Collapsed : Visibility.Visible;
        public PluginProperty PluginProperty { get; set; }

        public bool LastElement { get; set; }

        public PluginPropertyViewModel(PluginProperty pluginProperty)
        {
            PluginProperty = pluginProperty;
        }
    }
}