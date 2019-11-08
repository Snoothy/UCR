using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidWizards.UCR.Core.Models
{
    public class Filter
    {

        public string Name { get; set; }
        public bool Negative { get; set; }

        public static string GetShadowName(string name, int shadowCloneNumber)
        {
            return $"{name}-shadow-{shadowCloneNumber}";
        }
    }
}
