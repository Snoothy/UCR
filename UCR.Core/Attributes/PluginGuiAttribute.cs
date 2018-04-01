using System;

namespace HidWizards.UCR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PluginGuiAttribute : Attribute
    {
        private string name;
        private int rowOrder;
        private int columnOrder;

        public PluginGuiAttribute(string name)
        {
            this.name = name;
            rowOrder = 0;
            columnOrder = 0;
        }

        public virtual string Name => name;

        public virtual int RowOrder
        {
            get => rowOrder;
            set => rowOrder = value;
        }

        public virtual int ColumnOrder
        {
            get => columnOrder;
            set => columnOrder = value;
        }
    }
}
