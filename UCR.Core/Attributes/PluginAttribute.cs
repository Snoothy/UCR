using System;

namespace HidWizards.UCR.Core.Attributes
{
    public class PluginAttribute : Attribute
    {
        private string name;

        public PluginAttribute(string name)
        {
            this.name = name;
        }

        public virtual string Name => name;
    }
}
