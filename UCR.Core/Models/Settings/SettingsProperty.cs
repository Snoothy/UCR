using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Attributes;

namespace HidWizards.UCR.Core.Models.Settings
{
    public class SettingsProperty : INotifyPropertyChanged
    {
        public string Title => GetSettingAttribute().Title;
        public string Description => GetSettingAttribute().Description;
        public PropertyInfo PropertyInfo { get; }

        public dynamic Property
        {
            get => PropertyInfo.GetValue(typeof(SettingsCollection));
            set
            {
                PropertyInfo.SetValue(typeof(SettingsCollection), Convert.ChangeType(value, PropertyInfo.PropertyType, CultureInfo.InvariantCulture));
                OnPropertyChanged(PropertyInfo.Name);
            }
        }

        public SettingsProperty(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }

        private SettingAttribute GetSettingAttribute()
        {
            return ((SettingAttribute) Attribute.GetCustomAttribute(PropertyInfo, typeof(SettingAttribute)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
