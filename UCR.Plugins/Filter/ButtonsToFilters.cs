using System;
using System.Collections.Generic;
using System.ComponentModel;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Buttons to Filters", Group = "Filter", Description = "Change to one of three filter states (Plus default) using two buttons")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button 1")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button 2")]
    public class ButtonsToFilters : Plugin
    {
        private short[] _buttonStates = {0, 0};
        private Dictionary<int, string> _filterNames;
        private int _currentDirection = -1;

        [PluginGui("Default Filter name (Optional)")]
        public string DefaultFilterName { get; set; } = string.Empty;

        [PluginGui("Button 1 Filter name")]
        public string Filter1Name { get; set; } = string.Empty;

        [PluginGui("Button 2 Filter name")]
        public string Filter2Name { get; set; } = string.Empty;

        [PluginGui("Buttons 1+2 Filter name")]
        public string Filter12Name { get; set; } = string.Empty;

        public override void InitializeCacheValues()
        {
            _filterNames = new Dictionary<int, string> { { -1, DefaultFilterName }, { 0, Filter1Name }, { 1, Filter2Name }, { 2, Filter12Name } };
            ChangeState();
        }

        public override void Update(params short[] values)
        {
            _buttonStates = values;
            ChangeState();
        }

        private void ChangeState()
        {
            var newFilter = -1;
            if (_buttonStates[0] == 1 && _buttonStates[1] != 1)
            {
                newFilter = 0;
            }
            else if (_buttonStates[0] != 1 && _buttonStates[1] == 1)
            {
                newFilter = 1;
            }
            else if (_buttonStates[0] == 1 && _buttonStates[1] == 1)
            {
                newFilter = 2;
            }
            SetFilterActive(newFilter);
        }

        private void SetFilterActive(int direction)
        {
            SetFilterState(_currentDirection, false);
            _currentDirection = direction;
            SetFilterState(_currentDirection, true);
        }

        private void SetFilterState(int direction, bool state)
        {
            if (_filterNames[direction] != "")
            {
                WriteFilterState(_filterNames[direction], state);
            }
        }
    }
}
