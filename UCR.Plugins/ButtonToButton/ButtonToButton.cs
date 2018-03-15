using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Serialization;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Models.Plugin;

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
