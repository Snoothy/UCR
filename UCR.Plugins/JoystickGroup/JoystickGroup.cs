using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Models.Plugin;

namespace HidWizards.UCR.Plugins.JoystickGroup
{
    [Export(typeof(Plugin))]
    public class JoystickGroup : PluginGroup
    {
        private List<int> _inputSequenceValues;
        private string _inputSequence = "1,2,3,4";
        public string InputSequence
        {
            get { return _inputSequence; }
            set
            {
                SetIntListValues(_inputSequenceValues, value);
                _inputSequence = value;
            }
        }

        private List<int> _outputSequenceValues;
        private string _outputSequence = "1,2,3,4";
        public string OutputSequence
        {
            get { return _outputSequence; }
            set
            {
                SetIntListValues(_outputSequenceValues, value);
                _outputSequence = value;
            }
        }

        private List<Plugin> TempPlugins;

        public override string PluginName()
        {
            return "Joystick Group";
        }

        public JoystickGroup()
        {
            TempPlugins = new List<Plugin>();
            _inputSequenceValues = new List<int>();
            _outputSequenceValues = new List<int>();
        }

        public override void OnActivate()
        {
            TempPlugins.Clear();
            for (var index = 0; index < Math.Min(_inputSequenceValues.Count, _outputSequenceValues.Count); index++)
            {
                foreach (var plugin in Plugins)
                {
                    var newPlugin = plugin.Duplicate();
                    TempPlugins.Add(newPlugin);
                    foreach (var input in newPlugin.Inputs)
                    {
                        input.Plugin = this;
                        input.DeviceNumber = _inputSequenceValues[index];
                    }
                    foreach (var input in newPlugin.Outputs)
                    {
                        input.Plugin = this;
                        input.DeviceNumber = _outputSequenceValues[index];
                    }
                }
                
            }
        }

        public override List<DeviceBinding> GetInputs()
        {
            var inputs = new List<DeviceBinding>();

            foreach (var plugin in TempPlugins)
            {
                inputs.AddRange(plugin.GetInputs());
            }
            return inputs;
        }

        private void SetIntListValues(List<int> list, string value)
        {
            list.Clear();
            if (string.IsNullOrEmpty(value)) return;
            foreach (var i in value.Split(','))
            {
                int val;
                if(int.TryParse(i, out val)) list.Add(val - 1);
            }
            ContextChanged();
        }

    }
}
