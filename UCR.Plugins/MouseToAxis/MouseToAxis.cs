using System.Collections.Generic;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.MouseToAxis
{
    [Plugin("Mouse to Axis")]
    [PluginInput(DeviceBindingCategory.Delta, "Mouse axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class MouseToAxis: Plugin
    {

        public override void Update(List<long> values)
        {
            WriteOutput(0, values[0]*(Constants.AxisMaxValue/1000));
        }
    }
}
