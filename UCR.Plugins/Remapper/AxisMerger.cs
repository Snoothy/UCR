using System;
using System.Reflection;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis Merger", Group = "Axis", Description = "Merge two axes into one output axis")]
    [PluginInput(DeviceBindingCategory.Range, "Axis high")]
    [PluginInput(DeviceBindingCategory.Range, "Axis low")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
	[PluginSettingsGroup("Sensitivity", Group = "Sensitivity")]
    public class AxisMerger : Plugin
    {
        [PluginGui("Dead zone", Order = 3)]
        public int DeadZone { get; set; }

        [PluginGui("Mode", Order = 0)]
        public AxisMergerMode Mode { get; set; }

        [PluginGui("Invert high", Order = 1)]
        public bool InvertHigh { get; set; }

        [PluginGui("Invert low", Order = 2)]
        public bool InvertLow { get; set; }

        [PluginGui("Linear", Order = 1, Group = "Sensitivity")]
        public bool Linear { get; set; }

        [PluginGui("Percentage", Order = 0, Group = "Sensitivity")]
        public int Sensitivity { get; set; }

        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();
        private readonly SensitivityHelper _sensitivityHelper = new SensitivityHelper();

        public enum AxisMergerMode
        {
            Average,
            Greatest,
            Sum
        }

        public AxisMerger()
        {
            DeadZone = 0;
            Sensitivity = 100;
        }

        public override void InitializeCacheValues()
        {
            Initialize();
        }

        public override void Update(params short[] values)
        {
            var valueHigh = values[0];
            var valueLow = values[1];
            short valueOutput;

            if (InvertHigh) valueHigh = Functions.Invert(valueHigh);
            if (InvertLow) valueLow = Functions.Invert(valueLow);

            switch (Mode)
            {
                case AxisMergerMode.Average:
                    valueOutput = (short) ((valueHigh + valueLow) / 2);
                    break;
                case AxisMergerMode.Greatest:
                    valueOutput = (Functions.SafeAbs(valueHigh) > Functions.SafeAbs(valueLow)) ? valueHigh : valueLow;
                    break;
                case AxisMergerMode.Sum:
                    valueOutput = Functions.ClampAxisRange(valueHigh + valueLow);
                    break;
                default:
                    valueOutput = 0;
                    break;
            }

            if (DeadZone != 0)
            {
                valueOutput = _deadZoneHelper.ApplyRangeDeadZone(valueOutput);
            }

            if (Sensitivity != 100) valueOutput = _sensitivityHelper.ApplyRangeSensitivity(valueOutput);

            WriteOutput(0, valueOutput);
        }
        
        private void Initialize()
        {
            _deadZoneHelper.Percentage = DeadZone;
            _sensitivityHelper.Percentage = Sensitivity;
            _sensitivityHelper.IsLinear = Linear;
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
