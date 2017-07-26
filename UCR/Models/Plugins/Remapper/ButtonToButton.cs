using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Mapping;

namespace UCR.Models.Plugins.Remapper
{
    class ButtonToButton : Plugin
    {
        Binding Input { get; set; }
        Binding Output { get; set; }

        public ButtonToButton(Profile profile) : base(profile)
        {

        }

        public override void Initialize()
        {
            Input = InitializeMapping(BindingType.Input, InputChanged);
        }

        private void InputChanged(long value)
        {
            WriteOutput(Output, value);
        }
    }
}
