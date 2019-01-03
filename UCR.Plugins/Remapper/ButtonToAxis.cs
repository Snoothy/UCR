using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Button to Axis")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class ButtonToAxis : Plugin
    {
        [PluginGui("Axis on release", RowOrder = 0)] 
        public double Range { get; set; }

        [PluginGui("Initialize axis", RowOrder = 0, ColumnOrder = 1)]
        public bool Initialize { get; set; }

        [PluginGui("Axis when pressed", RowOrder = 1)]
        public double RangePressed { get; set; }

        public ButtonToAxis()
        {
            Range = 0;
            RangePressed = 100;
        }

        public override void OnActivate()
        {
            if (Initialize) WriteOutput(0, Functions.GetRangeFromPercentage((short)Range));
        }

        public override void Update(params short[] values)
        {
            WriteOutput(0,
                values[0] == 0
                    ? Functions.GetRangeFromPercentage((short)Range)
                    : Functions.GetRangeFromPercentage((short)RangePressed));
        }

    }
}
