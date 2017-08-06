using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Mapping;

namespace UCR.Models.Plugins
{
    class PluginGroup : Plugin
    {
        public List<Plugin> Plugins { get; set; }

        public PluginGroup()
        {
            Plugins = new List<Plugin>();
        }

        public override List<DeviceBinding> GetInputs()
        {
            List<DeviceBinding> newBindings = Inputs.Select(input => new DeviceBinding(input)).ToList();

            foreach (var plugin in Plugins)
            {
                newBindings.AddRange(plugin.GetInputs());
            }

            return newBindings;
        }
    }
}
