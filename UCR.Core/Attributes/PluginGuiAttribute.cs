using System;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PluginGuiAttribute : Attribute
    {
        private readonly string _name;
        private int _order;
        private string _group;

        public PluginGuiAttribute(string name)
        {
            _name = name;
            _order = 0;
            _group = null;
        }

        public virtual string Name => _name;

        public virtual int Order
        {
            get => _order;
            set => _order = value;
        }

        public virtual string Group
        {
            get => _group;
            set => _group = value;
        }
    }
}
