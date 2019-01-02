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
        [PluginGui("Axis on release", Order = 1)] 
        public double Range { get; set; }

        [PluginGui("Initialize axis", Order = 0)]
        public bool Initialize { get; set; }

        [PluginGui("Axis when pressed", Order = 2)]
        public double RangePressed { get; set; }

        public ButtonToAxis()
        {
            Range = 0;
            RangePressed = 100;
        }

        public override void OnActivate()
        {
            if (Initialize) WriteOutput(0, Functions.GetRangeFromPercentage((int)Range));
        }

        public override void Update(params long[] values)
        {
            WriteOutput(0,
                values[0] == 0
                    ? Functions.GetRangeFromPercentage((int)Range)
                    : Functions.GetRangeFromPercentage((int)RangePressed));
        }

    }
}
