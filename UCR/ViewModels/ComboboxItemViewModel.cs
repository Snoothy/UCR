using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCR.ViewModels
{
    public class ComboBoxItemViewModel
    {
        public string Title { get; set; }
        public dynamic Value { get; set; }

        public ComboBoxItemViewModel(string title, dynamic value)
        {
            Title = title;
            Value = value;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
