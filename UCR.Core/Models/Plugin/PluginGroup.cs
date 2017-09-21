using System.Collections.Generic;
using System.Linq;
using UCR.Core.Models.Device;

namespace UCR.Core.Models.Plugin
{
    public abstract class PluginGroup : Models.Plugin.Plugin
    {
        public List<Models.Plugin.Plugin> Plugins { get; set; }

        public PluginGroup()
        {
            Plugins = new List<Models.Plugin.Plugin>();
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
