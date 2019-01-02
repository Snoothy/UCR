using System;

namespace HidWizards.UCR.Core.Attributes
{
    public class PluginAttribute : Attribute
    {
        private string _name;
        private bool _disabled;

        public PluginAttribute(string name)
        {
            _name = name;
            _disabled = false;
        }

        public virtual string Name => _name;

        public virtual bool Disabled
        {
            get => _disabled;
            set => _disabled = value;
        }
    }
}
