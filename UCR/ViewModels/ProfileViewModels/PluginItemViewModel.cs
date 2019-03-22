using System.Windows;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class PluginItemViewModel
    {
        public string Name => Plugin.PluginName;
        public string Description => Plugin.Description;
        public Visibility SeparatorVisibility => FirstElement ? Visibility.Collapsed : Visibility.Visible;
        public bool FirstElement { get; set; }

        public Plugin Plugin { get; }

        public PluginItemViewModel(Plugin plugin)
        {
            Plugin = plugin;
        }
    }
}