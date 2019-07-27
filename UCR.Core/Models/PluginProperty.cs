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
        public int Order { get; }
        public string Group { get; }

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

        public PluginProperty(Plugin plugin, PropertyInfo propertyInfo, string name, int order = 0, string group = null)
        {
            Plugin = plugin;
            PropertyInfo = propertyInfo;
            Name = name;
            Order = order;
            Group = group;
        }

        public int CompareTo(PluginProperty other)
        {
            return Order.CompareTo(other.Order);
        }

        public PropertyValidationResult Validate(dynamic value)
        {
            return Plugin.Validate(PropertyInfo, value);
        }
    }
}
