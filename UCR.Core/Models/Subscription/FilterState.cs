using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidWizards.UCR.Core.Models.Subscription
{
    public class FilterState
    {
        public Dictionary<string, bool> FilterRuntimeDictionary { get; set; }

        public delegate void FilterStateChanged(string filterName, bool value);
        public event FilterStateChanged FilterStateChangedEvent;

        public FilterState()
        {
            FilterRuntimeDictionary = new Dictionary<string, bool>();
        }

        public void SetFilterState(string filterName, bool value)
        {
            var previousValue = FilterRuntimeDictionary[filterName];
            if (previousValue == value) return;

            FilterRuntimeDictionary[filterName] = value;
            FilterStateChangedEvent?.Invoke(filterName, value);
        }

        public void ToggleFilterState(string filterName)
        {
            FilterRuntimeDictionary[filterName] = !FilterRuntimeDictionary[filterName];
            FilterStateChangedEvent?.Invoke(filterName, FilterRuntimeDictionary[filterName]);
        }
    }
}
