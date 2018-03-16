using System;
using System.ComponentModel.Composition;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.ButtonToAxis
{
    [Export(typeof(Plugin))]
    public class ButtonToAxis : Plugin
    {

        private long _direction = 0;

        public override string PluginName()
        {
            return "Button to Axis";
        }

        public ButtonToAxis()
        {

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
