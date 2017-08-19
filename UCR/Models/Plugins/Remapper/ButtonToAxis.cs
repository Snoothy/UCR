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
        public DeviceBinding Input { get; set; }
        public DeviceBinding Output { get; set; }

        public ButtonToAxis() { }

        public ButtonToAxis(Profile parentProfile) : base(parentProfile)
        {
            Input = InitializeInputMapping(InputChanged);
            Output = InitializeOutputMapping();
        }

        private void InputChanged(long value)
        {
            WriteOutput(Output, value);
        }
    }
}
