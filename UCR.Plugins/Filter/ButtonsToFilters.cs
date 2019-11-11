using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Buttons to Filters", Group = "Filter", Description = "Change to one of three filter states using two buttons")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button 1")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button 2")]
    public class ButtonsToFilters : Plugin
    {
        private short[] _buttonStates = {0, 0};

        [PluginGui("Default Filter name (Optional)")]
        public string DefaultFilterName { get; set; } = string.Empty;

        [PluginGui("Button 1 Filter name")]
        public string Filter1Name { get; set; }

        [PluginGui("Button 2 Filter name")]
        public string Filter2Name { get; set; }

        [PluginGui("Buttons 1+2 Filter name")]
        public string Filter12Name { get; set; }

        public ButtonsToFilters()
        {
        }

        public override void InitializeCacheValues()
        {
            ChangeState();
        }

        public override void Update(params short[] values)
        {
            _buttonStates = values;
            ChangeState();
        }

        private void ChangeState()
        {
            if (_buttonStates[0] == 1 && _buttonStates[1] != 1)
            {
                if (DefaultFilterName != "")
                {
                    WriteFilterState(DefaultFilterName, false);
                }
                WriteFilterState(Filter12Name, false);
                WriteFilterState(Filter2Name, false);
                WriteFilterState(Filter1Name, true);
            }
            else if (_buttonStates[0] != 1 && _buttonStates[1] == 1)
            {
                if (DefaultFilterName != "")
                {
                    WriteFilterState(DefaultFilterName, false);
                }
                WriteFilterState(Filter1Name, false);
                WriteFilterState(Filter12Name, false);
                WriteFilterState(Filter2Name, true);
            }
            else if (_buttonStates[0] == 1 && _buttonStates[1] == 1)
            {
                if (DefaultFilterName != "")
                {
                    WriteFilterState(DefaultFilterName, false);
                }
                WriteFilterState(Filter1Name, false);
                WriteFilterState(Filter2Name, false);
                WriteFilterState(Filter12Name, true);
            }
            else
            {
                WriteFilterState(Filter1Name, false);
                WriteFilterState(Filter2Name, false);
                WriteFilterState(Filter12Name, false);
                if (DefaultFilterName != "")
                {
                    WriteFilterState(DefaultFilterName, true);
                }
            }
        }
    }
}
