using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class SimplePluginViewModel
    {
        public string Name => Plugin.PluginName;
        public string Description => Plugin.Description;
        public Plugin Plugin { get; set; }

        public SimplePluginViewModel(Plugin plugin)
        {
            Plugin = plugin;
        }
    }
}
