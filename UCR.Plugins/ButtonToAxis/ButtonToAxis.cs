using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.ButtonToAxis
{
    [Export(typeof(Plugin))]
    public class ButtonToAxis : Plugin
    {
        public override string PluginName => "Button to axis";
        public override DeviceBindingCategory OutputCategory => DeviceBindingCategory.Range;
        protected override List<PluginInput> InputCategories => new List<PluginInput>()
        {
            new PluginInput()
            {
                Name = "Button",
                Category = DeviceBindingCategory.Momentary
            }
        };

        private long _direction = 0;

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
