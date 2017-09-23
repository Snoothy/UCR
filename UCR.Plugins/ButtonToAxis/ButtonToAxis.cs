using System;
using System.ComponentModel.Composition;
using System.Xml.Serialization;
using UCR.Core.Models.Binding;
using UCR.Core.Models.Device;
using UCR.Core.Models.Plugin;
using UCR.Core.Utilities;

namespace UCR.Plugins.ButtonToAxis
{
    [Export(typeof(Plugin))]
    public class ButtonToAxis : Plugin
    {
        [XmlIgnore]
        public DeviceBinding InputHigh { get; set; }
        [XmlIgnore]
        public DeviceBinding InputLow { get; set; }
        [XmlIgnore]
        public DeviceBinding Output { get; set; }

        private long _direction = 0;

        public override string PluginName()
        {
            return "Button to Axis";
        }

        public ButtonToAxis()
        {
            InputLow = InitializeInputMapping(InputLowChanged);
            InputHigh = InitializeInputMapping(InputHighChanged);
            Output = InitializeOutputMapping();
        }

        private void InputLowChanged(long value)
        {
            _direction += value == 0 ? 1 : -1;
            WriteOutput();
        }

        private void InputHighChanged(long value)
        {
            _direction += value == 0 ? -1 : 1;
            WriteOutput();
        }

        private void WriteOutput()
        {
            _direction = Math.Sign(_direction);
            WriteOutput(Output, _direction*Constants.AxisMaxValue);
        }
    }
}
