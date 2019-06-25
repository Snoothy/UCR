using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Button to Axis", Group = "Axis", Description = "Map from one button to different axis values")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis", Group = "Axis")]
    public class ButtonToAxis : Plugin
    {
        [PluginGui("Axis % on press (-100..100)", Group = "Axis")]
        public double RangePressed { get; set; }

        [PluginGui("Axis % on release (-100..100)", Group = "Axis")]
        public double Range { get; set; }

        [PluginGui("Initialize to release value on activate")]
        public bool Initialize { get; set; }

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
