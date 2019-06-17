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
        [PluginGui("Axis on release", Order = 0, Group = "Axis")] 
        public double Range { get; set; }

        [PluginGui("Initialize axis", Order = 0)]
        public bool Initialize { get; set; }

        [PluginGui("Axis when pressed", Order = 1, Group = "Axis")]
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
