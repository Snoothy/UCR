using System;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginSettingsGroupAttribute : PluginGroupAttribute
    {
        public PluginSettingsGroupAttribute(string name)
        {
            Name = name;
        }

        public virtual string Name { get; }
    }
}