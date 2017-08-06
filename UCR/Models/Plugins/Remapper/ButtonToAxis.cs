using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Mapping;
using UCR.Views.Controls;

namespace UCR.Models.Plugins.Remapper
{
    class ButtonToAxis : Plugin
    {
        DeviceBinding Input { get; set; }
        DeviceBinding Output { get; set; }

        public ButtonToAxis() { }

        public ButtonToAxis(Profile profile) : base(profile)
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
