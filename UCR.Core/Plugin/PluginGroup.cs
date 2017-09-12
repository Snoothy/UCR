using System.Collections.Generic;
using System.Linq;
using UCR.Core.Device;

namespace UCR.Core.Plugin
{
    public abstract class PluginGroup : Plugin
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
