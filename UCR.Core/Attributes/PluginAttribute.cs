using System;

namespace HidWizards.UCR.Core.Attributes
{
    public class PluginAttribute : Attribute
    {
        private string name;
        private bool disabled;

        public PluginAttribute(string name)
        {
            this.name = name;
            disabled = false;
        }

        public virtual string Name => name;

        public virtual bool Disabled
        {
            get => disabled;
            set => disabled = value;
        }
    }
}
