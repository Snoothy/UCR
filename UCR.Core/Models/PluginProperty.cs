using System;
using System.CodeDom;
using System.Globalization;
using System.Reflection;

namespace HidWizards.UCR.Core.Models
{
    public class PluginProperty : IComparable<PluginProperty>
    {
        public string Name { get; }
        public Plugin Plugin { get; }
        public int RowOrder { get; }
        public int ColumnOrder { get; }

        public PropertyInfo PropertyInfo { get; }
        public dynamic Property
        {
            get => PropertyInfo.GetValue(Plugin);
            set
            {
                if (value.Equals(PropertyInfo.GetValue(Plugin))) return;
                PropertyInfo.SetValue(Plugin, Convert.ChangeType(value, PropertyInfo.PropertyType, CultureInfo.InvariantCulture));
                if (Plugin.Profile.IsActive())
                {
                    Plugin.InitializeCacheValues();
                    Plugin.OnPropertyChanged();
                }
                Plugin.ContextChanged();
            }
        }

        public PluginProperty(Plugin plugin, PropertyInfo propertyInfo, string name, int rowOrder = 0, int columnOrder = 0)
        {
            Plugin = plugin;
            PropertyInfo = propertyInfo;
            Name = name;
            RowOrder = rowOrder;
            ColumnOrder = columnOrder;
        }

        public int CompareTo(PluginProperty other)
        {
            return RowOrder.CompareTo(other.RowOrder);
        }
    }
}
