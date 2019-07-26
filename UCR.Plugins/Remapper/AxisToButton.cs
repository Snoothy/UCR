using System;
using System.Reflection;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to Button", Group = "Button", Description = "Map from one axis to two buttons")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button high")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button low")]
    public class AxisToButton : Plugin
    {
        [PluginGui("Invert", Order = 0)]
        public bool Invert { get; set; }

        [PluginGui("Dead zone", Order = 1)]
        public int DeadZone { get; set; }

        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();

        public AxisToButton()
        {
            DeadZone = 30;
        }

        public override void InitializeCacheValues()
        {
            Initialize();
        }

        public override void Update(params short[] values)
        {
            var value = values[0];
            if (Invert) value = Functions.Invert(value);
            switch (Math.Sign(_deadZoneHelper.ApplyRangeDeadZone(value)))
            {
                case 0:
                    WriteOutput(0, 0);
                    WriteOutput(1, 0);
                    break;
                case 1:
                    WriteOutput(0, 1);
                    WriteOutput(1, 0);
                    break;
                case -1:
                    WriteOutput(0, 0);
                    WriteOutput(1, 1);
                    break;
            }
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
