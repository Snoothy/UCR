using System;
using UCR.Models.Mapping;

namespace UCR.Models.Plugins.Remapper
{
    public class ButtonToAxis : Plugin
    {
        public DeviceBinding InputHigh { get; set; }
        public DeviceBinding InputLow { get; set; }
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
            WriteOutput(Output, _direction*UCRConstants.AxisMaxValue);
        }
    }
}
