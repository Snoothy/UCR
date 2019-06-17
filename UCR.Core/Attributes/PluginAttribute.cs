using System;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; set; }
        public string Group { get; set; }
        public bool Disabled { get; set; }

        public PluginAttribute(string name)
        {
            Name = name;
        }
    }
}
