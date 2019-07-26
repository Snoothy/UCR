using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Button to Event", Group = "Event", Description = "Remap a Button to an Event")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Event, "Event")]
    public class ButtonToEvent : Plugin
    {
        public enum ButtonToEventMode { Press, Release}

        [PluginGui("Perform action on", Order = 0)]
        public ButtonToEventMode Mode { get; set; }

        public ButtonToEvent()
        {
            Mode = ButtonToEventMode.Press;
        }

        public override void Update(params short[] values)
        {
            if (values[0] == 1 && Mode == ButtonToEventMode.Press || values[0] == 0 && Mode == ButtonToEventMode.Release)
            {
                WriteOutput(0, 1);
            }
        }
    }
}
