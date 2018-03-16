using System.Collections.Generic;
using System.ComponentModel.Composition;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.Plugins.ButtonToButton
{
    [Export(typeof(Plugin))]
    public class ButtonToButton : Plugin
    {

        public override string PluginName()
        {
            return "Button to Button";
        }

        public override long Update(List<long> values)
        {
            return values[0];
        }
    }
}
