using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis Initializer", Disabled = true)]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisInitializer : Plugin
    {
        [PluginGui("Percentage")]
        public decimal Percentage { get; set; }

        public AxisInitializer()
        {
        }
        
        public override void InitializeCacheValues()
        {
            Initialize();
        }

        public override void Update(params short[] values)
        {
        }

        private void Initialize()
        {
            var value = (int)((Percentage / 100) * Constants.AxisMaxAbsValue);
            value = Functions.ClampAxisRange(value);
            WriteOutput(0, (short) value);
        }
    }
}
