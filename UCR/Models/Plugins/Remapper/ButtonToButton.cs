using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Mapping;
using UCR.Views.Controls;

namespace UCR.Models.Plugins.Remapper
{
    class ButtonToButton : Plugin
    {
        Binding Input { get; set; }
        Binding Output { get; set; }

        public ButtonToButton(Profile profile) : base(profile)
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
