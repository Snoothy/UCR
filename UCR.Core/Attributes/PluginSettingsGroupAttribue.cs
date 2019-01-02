using System;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginSettingsGroupAttribute : PluginGroupAttribute
    {
        private string _name;

        public PluginSettingsGroupAttribute(string name, string group) : base(group)
        {
            _name = name;
        }

        public virtual string Name => _name;
    }
}
