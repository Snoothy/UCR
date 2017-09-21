using System.ComponentModel.Composition;
using UCR.Core.Models.Plugin;

namespace UCR.Plugins.AxisToAxis
{
    [Export(typeof(Plugin))]
    public class AxisToAxis : Plugin
    {
        public AxisToAxis()
        {
            
        }

        public override string PluginName()
        {
            return "Axis to axis";
        }
    }
}
