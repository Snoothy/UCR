using System.Collections.Generic;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.MouseToAxis
{
    [Plugin("Mouse to Axis", Disabled = true)]
    [PluginInput(DeviceBindingCategory.Delta, "Mouse axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class MouseToAxis: Plugin
    {
        [PluginGui("Invert", RowOrder = 0, ColumnOrder = 0)]
        public bool Invert { get; set; }

        public override void Update(params long[] values)
        {
            WriteOutput(0, values[0]*(Constants.AxisMaxValue/1000));
        }
    }
}
