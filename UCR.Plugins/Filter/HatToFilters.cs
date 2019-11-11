using System;
using System.Collections.Generic;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Hat Filters", Group = "Filter", Description = "Set one of four filter states using a Hat Switch / D-Pad")]
    [PluginInput(DeviceBindingCategory.Momentary, "Direction 1")]
    [PluginInput(DeviceBindingCategory.Momentary, "Direction 2")]
    [PluginInput(DeviceBindingCategory.Momentary, "Direction 3")]
    [PluginInput(DeviceBindingCategory.Momentary, "Direction 4")]
    public class HatToFilters : Plugin
    {
        private short[] _buttonStates = {0, 0, 0, 0};
        private Dictionary<int, string> _filterNames;
        private int _currentDirection = -1;

        [PluginGui("Default Filter name (Optional)")]
        public string DefaultFilterName { get; set; } = string.Empty;

        [PluginGui("Direction 1 Filter name")]
        public string Filter1Name { get; set; }

        [PluginGui("Direction 2 Filter name")]
        public string Filter2Name { get; set; }

        [PluginGui("Direction 3 Filter name")]
        public string Filter3Name { get; set; }

        [PluginGui("Direction 4 Filter name")]
        public string Filter4Name { get; set; }

        public override void InitializeCacheValues()
        {
            _filterNames = new Dictionary<int, string> { { -1, DefaultFilterName} , { 0, Filter1Name}, { 1, Filter2Name}, { 2, Filter3Name}, {3, Filter4Name} };
            ChangeState();
        }

        public override void Update(params short[] values)
        {
            _buttonStates = values;
            ChangeState();
        }

        private void ChangeState()
        {
            var direction = -1;
            for (var i = 0; i < 4; i++)
            {
                if (direction != -1 && _buttonStates[i] == 1)
                {
                    direction = -1;
                    break;
                }

                if (_buttonStates[i] == 1)
                {
                    direction = i;
                }
            }

            SetFilterActive(direction);
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
