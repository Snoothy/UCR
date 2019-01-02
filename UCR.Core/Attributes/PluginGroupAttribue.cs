using System;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginGroupAttribute : Attribute
    {
        private string _groupName;

        public PluginGroupAttribute(string groupName)
        {
            _groupName = groupName;
        }

        public virtual string GroupName => _groupName;
    }
}
