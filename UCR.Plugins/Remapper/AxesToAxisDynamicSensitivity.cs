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
    [Plugin("Axes to Axis (Dynamic Sensitivity)", Group = "Axis", Description = "Map from one axis to another and use a 2nd axis to control sensitivity")]
    [PluginInput(DeviceBindingCategory.Range, "Input Axis")]
    [PluginInput(DeviceBindingCategory.Range, "Sensitivity Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    [PluginSettingsGroup("Sensitivity", Group = "Sensitivity")]
    [PluginSettingsGroup("Dead zone", Group = "Dead zone")]
    public class AxesToAxisDynamicSensitivity : Plugin
    {
        [PluginGui("Invert Input Axis")]
        public bool Invert { get; set; }

        [PluginGui("Percentage", Group = "Dead zone", Order = 0)]
        public int DeadZone { get; set; }

        [PluginGui("Anti-dead zone", Group = "Dead zone")]
        public int AntiDeadZone { get; set; }

        [PluginGui("Linear", Group = "Sensitivity", Order = 1)]
        public bool Linear { get; set; }

        [PluginGui("Invert Sensitivity Axis", Group = "Sensitivity", Order = 2)]
        public bool InvertSensitivity { get; set; }

        [PluginGui("Sensitivity Min", Group = "Sensitivity", Order = 3)]
        public double SensitivityMin { get; set; }

        [PluginGui("Sensitivity Max", Group = "Sensitivity", Order = 4)]
        public double SensitivityMax { get; set; }

        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();
        private readonly AntiDeadZoneHelper _antiDeadZoneHelper = new AntiDeadZoneHelper();
        private readonly SensitivityHelper _sensitivityHelper = new SensitivityHelper();
        private short _currentSensitivityAxisValue;

        public AxesToAxisDynamicSensitivity()
        {
            DeadZone = 0;
            AntiDeadZone = 0;
            SensitivityMin = 0;
            SensitivityMax = 100;
        }

        public override void InitializeCacheValues()
        {
            Initialize();
        }

        public override void Update(params short[] values)
        {
            var value = values[0];
            if (Invert) value = Functions.Invert(value);
            if (DeadZone != 0) value = _deadZoneHelper.ApplyRangeDeadZone(value);
            if (AntiDeadZone != 0) value = _antiDeadZoneHelper.ApplyRangeAntiDeadZone(value);
            if (values[1] != _currentSensitivityAxisValue)
            {
                _currentSensitivityAxisValue = values[1];
                RecalculateSensitivity(values[1]);
            }
            //Debug.WriteLine($"UCR| Axis value: {values[0]}, Sens value: {values[1]}, Sens %: {_sensitivityHelper.Percentage}");
            value = _sensitivityHelper.ApplyRangeSensitivity(value);
            WriteOutput(0, value);
        }

        private void Initialize()
        {
            _deadZoneHelper.Percentage = DeadZone;
            _antiDeadZoneHelper.Percentage = AntiDeadZone;
            _sensitivityHelper.IsLinear = Linear;
        }

        private void RecalculateSensitivity(short sensValue)
        {
            if (InvertSensitivity)
            {
                sensValue = Functions.Invert(sensValue);
            }

            var sensitivityUnits = SensitivityMax - SensitivityMin;
            var sensPercentPerUnit = sensitivityUnits / 100D;
            var percent = SensitivityMin + (Functions.GetPercentageFromRange(sensValue) * sensPercentPerUnit );
            _sensitivityHelper.Percentage = (int)percent;
        }

        public override PropertyValidationResult Validate(PropertyInfo propertyInfo, dynamic value)
        {
            switch (propertyInfo.Name)
            {
                case nameof(DeadZone):
                case nameof(AntiDeadZone):
                    return InputValidation.ValidatePercentage(value);
                case nameof(SensitivityMin):
                    return new PropertyValidationResult(value < SensitivityMax, "Min must be less than max");
                case nameof(SensitivityMax):
                    return new PropertyValidationResult(value > SensitivityMin, "Max must be more than min");
            }
            
            return PropertyValidationResult.ValidResult;
        }
    }
}
