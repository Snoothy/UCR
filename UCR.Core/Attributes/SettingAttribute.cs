using System;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingAttribute : Attribute
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
