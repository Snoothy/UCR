using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCR.ViewModels
{
    public class ComboBoxItemViewModel
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public ComboBoxItemViewModel(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
