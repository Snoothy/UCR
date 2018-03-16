using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.ButtonToAxis
{
    [Export(typeof(Plugin))]
    public class ButtonToAxis : Plugin
    {

        private long _direction = 0;

        public override string PluginName()
        {
            return "Button to Axis";
        }

        public ButtonToAxis()
        {

        }

        // TODO Implement value to set 
        public override long Update(List<long> values)
        {
            return values[0] * Constants.AxisMaxValue;
        }
    }
}
