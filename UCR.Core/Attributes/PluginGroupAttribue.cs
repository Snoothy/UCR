using System;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginGroupAttribute : Attribute
    {
        public virtual string Name { get; set; }
        public string Group { get; set; }

        public PluginGroupAttribute()
        {
        }
    }
}
