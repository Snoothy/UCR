using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.ButtonToState
{
    [Plugin("Button to state")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    public class ButtonToState : Plugin
    {

        [PluginGui("Invert", ColumnOrder = 0, RowOrder = 0)]
        public bool Invert { get; set; }

        public override void Update(params long[] values)
        {
            long value;
            var guid = Guid.Parse("2f9ec6c0-18f6-4a8d-a432-95a64a26814a");
            if (Invert)
            {
                value = values[0] == 0 ? 1 : 0;
            }
            else
            {
                value = values[0];
            }

            SetState(guid, value != 0);
        }
    }
}
