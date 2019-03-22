using System;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginGroupAttribute : Attribute
    {
        public string Group { get; set; }

        public PluginGroupAttribute()
        {
        }
    }
}
