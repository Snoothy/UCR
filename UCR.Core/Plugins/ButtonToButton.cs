using System;
using System.Xml.Serialization;
using UCR.Core.Device;

namespace UCR.Core.Plugins
{
    public class ButtonToButton : Plugin.Plugin
    {
        [XmlIgnore]
        public DeviceBinding Input { get; set; }
        [XmlIgnore]
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
