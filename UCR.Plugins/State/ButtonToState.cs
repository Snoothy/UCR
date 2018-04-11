using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.State
{
    [Plugin("Button to state")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    public class ButtonToState : Plugin
    {

        [PluginGui("Invert", ColumnOrder = 1, RowOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("State", ColumnOrder = 0, RowOrder = 0)]
        public Guid StateGuid { get; set; }

        public override void Update(params long[] values)
        {
            long value;
            if (Invert)
            {
                value = values[0] == 0 ? 1 : 0;
            }
            else
            {
                value = values[0];
            }

            SetState(StateGuid, value != 0);
        }
    }
}
