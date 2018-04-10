using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.AxisSplitter
{
    [Plugin("Axis splitter")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis high")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis low")]
    public class AxisSplitter : Plugin
    {
        [PluginGui("Invert high", RowOrder = 1)]
        public bool InvertHigh { get; set; }

        [PluginGui("Invert low", RowOrder = 2)]
        public bool InvertLow { get; set; }

        [PluginGui("Dead zone")]
        public int DeadZone { get; set; }

        public AxisSplitter()
        {
            DeadZone = 0;
        }

        public override void Update(params long[] values)
        {
            var value = values[0];

            if (DeadZone != 0) value = Functions.ApplyRangeDeadZone(value, DeadZone);
            WriteOutput(0, Functions.HalfAxisToFullRange(value, true, InvertHigh));
            WriteOutput(1, Functions.HalfAxisToFullRange(value, false, InvertLow));
        }
    }
}
