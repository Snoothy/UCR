using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Button to Filter", Group = "Filter", Description = "Change filter state using a button")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    public class ButtonToFilter : Plugin
    {
        [PluginGui("Filter name")]
        public string FilterName { get; set; }

        [PluginGui("Button down filter state")]
        public FilterState FilterStateDown { get; set; }

        [PluginGui("Button up filter state")]
        public FilterState FilterStateUp { get; set; }

        public enum FilterState
        {
            Active,
            Inactive,
            Toggle,
            Unchanged
        }

        public ButtonToFilter()
        {
            FilterStateDown = FilterState.Active;
            FilterStateUp = FilterState.Inactive;
        }

        public override void Update(params short[] values)
        {
            ChangeState(values[0] == 0 ? FilterStateUp : FilterStateDown);
        }

        private void ChangeState(FilterState filterState)
        {
            switch (filterState)
            {
                case FilterState.Active:
                    WriteFilterState(FilterName, true);
                    break;
                case FilterState.Inactive:
                    WriteFilterState(FilterName, false);
                    break;
                case FilterState.Toggle:
                    ToggleFilterState(FilterName);
                    break;
                case FilterState.Unchanged:
                    break;
            }
        }
    }
}
