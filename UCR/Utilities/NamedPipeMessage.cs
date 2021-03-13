using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidWizards.UCR.Utilities
{
    [Serializable]
    public class NamedPipeMessage
    {

        public List<string> Commands { get; set; }

    }
}
