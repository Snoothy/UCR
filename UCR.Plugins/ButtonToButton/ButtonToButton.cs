using System;
using System.ComponentModel.Composition;
using System.Xml.Serialization;
using UCR.Core.Models.Device;
using UCR.Core.Models.Plugin;

namespace UCR.Plugins.ButtonToButton
{
    [Export(typeof(Plugin))]
    public class ButtonToButton : Plugin
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
