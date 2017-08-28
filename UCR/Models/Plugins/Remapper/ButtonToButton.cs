using System;
using UCR.Models.Mapping;

namespace UCR.Models.Plugins.Remapper
{
    public class ButtonToButton : Plugin
    {
        public DeviceBinding Input { get; set; }
        public DeviceBinding Output { get; set; }

        public override string PluginName()
        {
            return "Button to Button";
        }

        public ButtonToButton()
        {
            Input = InitializeInputMapping(InputChanged);
            Output = InitializeOutputMapping();
        }

        private void InputChanged(long value)
        {
            WriteOutput(Output, Math.Max(Math.Min(value, 1),0));
        }
    }
}
