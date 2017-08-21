using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Mapping;
using UCR.Views.Controls;

namespace UCR.Models.Plugins.Remapper
{
    public class ButtonToAxis : Plugin
    {
        public DeviceBinding InputHigh { get; set; }
        public DeviceBinding InputLow { get; set; }
        public DeviceBinding Output { get; set; }

        private long direction = 0;

        public ButtonToAxis()
        {
            InputLow = InitializeInputMapping(InputLowChanged);
            InputHigh = InitializeInputMapping(InputHighChanged);
            Output = InitializeOutputMapping();
        }

        private void InputLowChanged(long value)
        {
            direction += value == 0 ? 1 : -1;
            WriteOutput();
        }

        private void InputHighChanged(long value)
        {
            direction += value == 0 ? -1 : 1;
            WriteOutput();
        }

        private void WriteOutput()
        {
            direction = Math.Sign(direction);
            WriteOutput(Output, direction*UCRConstants.AxisMaxValue);
        }
    }
}
