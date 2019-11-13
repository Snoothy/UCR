using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Plugins.Filter
{
    [Plugin("Button to Filter", Group = "Filter", Description = "Change filter state using a button")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    public class ButtonToFilter : Plugin
    {
        [PluginGui("Filter name")]
        public string FilterName { get; set; }

        [PluginGui("Button down filter state")]
        public FilterMode FilterStateDown { get; set; }

        [PluginGui("Button up filter state")]
        public FilterMode FilterStateUp { get; set; }


        public ButtonToFilter()
        {
            FilterStateDown = FilterMode.Active;
            FilterStateUp = FilterMode.Inactive;
        }

        public override void Update(params short[] values)
        {
            ChangeState(values[0] == 0 ? FilterStateUp : FilterStateDown);
        }

        private void ChangeState(FilterMode filterState)
        {
            switch (filterState)
            {
                case FilterMode.Active:
                    WriteFilterState(FilterName, true);
                    break;
                case FilterMode.Inactive:
                    WriteFilterState(FilterName, false);
                    break;
                case FilterMode.Toggle:
                    ToggleFilterState(FilterName);
                    break;
                case FilterMode.Unchanged:
                    break;
            }
        }
    }
}
