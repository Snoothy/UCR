using System;
using System.Diagnostics;
using System.Reflection;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to Button", Group = "Button", Description = "Map from one pedal style axis to a button, with press and release threshold")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button")]
    public class AxisToButton : Plugin
    {
        [PluginGui("Invert", Order = 0)]
        public bool Invert { get; set; }

        [PluginGui("Press at axis value", Order = 1)]
        public double PressPercent { get; set; }

        [PluginGui("Release at axis value", Order = 2)]
        public double ReleasePercent { get; set; }

        private bool _pressed;
        private double _pressThresh;
        private double _releaseThresh;

        public AxisToButton()
        {
            PressPercent = -80;
            ReleasePercent = -100;
        }

        public override void InitializeCacheValues()
        {
            Initialize();
            _pressThresh = Functions.GetRangeFromPercentage(PressPercent);
            _releaseThresh = Functions.GetRangeFromPercentage(ReleasePercent);
        }

        public override void Update(params short[] values)
        {
            var value = values[0];
            if (Invert) value = Functions.Invert(value);
            //Debug.WriteLine($"Current: {value}, Press @: {_pressThresh} Release @: {_releaseThresh}, Pressed: {_pressed}");
            if (_pressed)
            {
                if (value <= _releaseThresh)
                {
                    _pressed = false;
                    WriteOutput(0, 0);
                }
            }
            else
            {
                if (value >= _pressThresh)
                {
                    _pressed = true;
                    WriteOutput(0, 1);
                }
            }
        }

        private void Initialize()
        {
        }

        public override PropertyValidationResult Validate(PropertyInfo propertyInfo, dynamic value)
        {
            switch (propertyInfo.Name)
            {
                case nameof(PressPercent):
                    if (value < ReleasePercent)
                    {
                        return new PropertyValidationResult(false, "Press must be higher or equal to Release");
                    }
                    return InputValidation.ValidateSignedPercentage(value);
                case nameof(ReleasePercent):
                    if (value > PressPercent)
                    {
                        return new PropertyValidationResult(false, "Release must be lower or equal to Press");
                    }
                    return InputValidation.ValidateSignedPercentage(value);
            }

            return PropertyValidationResult.ValidResult;
        }
    }
}
