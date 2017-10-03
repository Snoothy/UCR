using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Core.Models.Plugin;

namespace UCR.Utilities
{
    public class DummyGroup : PluginGroup
    {
        public override string PluginName()
        {
            return "Dummy plugin group";
        }
    }
}
