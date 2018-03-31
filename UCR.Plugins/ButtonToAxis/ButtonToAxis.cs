using System.Collections.Generic;
using System.ComponentModel.Composition;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.ButtonToAxis
{
    [Plugin("Button to axis")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class ButtonToAxis : Plugin
    {
        private long _direction = 0;

        public ButtonToAxis()
        {

        }

        // TODO Implement value to set 
        public override void Update(List<long> values)
        {
            WriteOutput(0, values[0] * Constants.AxisMaxValue);
        }
    }
}
