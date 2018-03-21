using System.Collections.Generic;
using System.ComponentModel.Composition;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.MouseToAxis
{
    [Export(typeof(Plugin))]
    public class MouseToAxis: Plugin
    {
        public override string PluginName => "Mouse to Axis";
        public override DeviceBindingCategory OutputCategory => DeviceBindingCategory.Range;
        protected override List<PluginInput> InputCategories => new List<PluginInput>()
        {
            new PluginInput()
            {
                Name = "Mouse axis",
                Category = DeviceBindingCategory.Delta
            }
        };

        public override long Update(List<long> values)
        {
            return values[0]*(Constants.AxisMaxValue/1000);
        }
    }
}
