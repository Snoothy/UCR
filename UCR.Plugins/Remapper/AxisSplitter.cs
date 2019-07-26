using System.Reflection;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis Splitter", Group = "Axis", Description = "Split one axis into two new axes")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis high")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis low")]
    public class AxisSplitter : Plugin
    {
        [PluginGui("Invert high", Order = 1)]
        public bool InvertHigh { get; set; }

        [PluginGui("Invert low", Order = 2)]
        public bool InvertLow { get; set; }

        [PluginGui("Dead zone")]
        public int DeadZone { get; set; }

        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();

        public AxisSplitter()
        {
            DeadZone = 0;
        }

        public override void InitializeCacheValues()
        {
            Initialize();
        }

        public override void Update(params short[] values)
        {
            var value = values[0];

            if (DeadZone != 0) value = _deadZoneHelper.ApplyRangeDeadZone(value);

            var high = Functions.SplitAxis(value, true);
            var low = Functions.SplitAxis(value, false);
            if (InvertHigh) high = Functions.Invert(high);
            if (InvertLow) low = Functions.Invert(low);
            WriteOutput(0, high);
            WriteOutput(1, low);
        }

        private void Initialize()
        {
            _deadZoneHelper.Percentage = DeadZone;
        }

        public override PropertyValidationResult Validate(PropertyInfo propertyInfo, dynamic value)
        {
            switch (propertyInfo.Name)
            {
                case nameof(DeadZone):
                    return InputValidation.ValidatePercentage(value);
            }

            return PropertyValidationResult.ValidResult;
        }
    }
}
