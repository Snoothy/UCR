using System;
using System.Reflection;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axes to Axes", Group = "Axis", Description = "Map from joystick to joystick")]
    [PluginInput(DeviceBindingCategory.Range, "X Axis")]
    [PluginInput(DeviceBindingCategory.Range, "Y Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "X Axis", Group = "X axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Y Axis", Group = "Y axis")]
    [PluginSettingsGroup("Sensitivity", Group = "Sensitivity")]
    [PluginSettingsGroup("Dead zone", Group = "Dead zone")]
    public class AxesToAxes : Plugin
    {
        private readonly CircularDeadZoneHelper _circularDeadZoneHelper = new CircularDeadZoneHelper();
        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();
        private readonly SensitivityHelper _sensitivityHelper = new SensitivityHelper();
        private double _linearSensitivityScaleFactor;

        [PluginGui("Invert X", Group = "X axis")]
        public bool InvertX { get; set; }

        [PluginGui("Invert Y", Group = "Y axis")]
        public bool InvertY { get; set; }

        [PluginGui("Percentage", Order = 0, Group = "Sensitivity")]
        public int Sensitivity { get; set; }

        [PluginGui("Linear", Order = 0, Group = "Sensitivity")]
        public bool Linear { get; set; }

        [PluginGui("Percentage", Order = 0, Group = "Dead zone")]
        public int DeadZone { get; set; }

        [PluginGui("Circular", Order = 1, Group = "Dead zone")]
        public bool CircularDz { get; set; }


        public AxesToAxes()
        {
            DeadZone = 0;
            Sensitivity = 100;
        }

        public override void InitializeCacheValues()
        {
            Initialize();
        }

        private void Initialize()
        {
            _deadZoneHelper.Percentage = DeadZone;
            _circularDeadZoneHelper.Percentage = DeadZone;
            _sensitivityHelper.Percentage = Sensitivity;
            _linearSensitivityScaleFactor = ((double)Sensitivity / 100);
        }

        public override void Update(params short[] values)
        {
            var outputValues = new short[] {values[0], values[1]};
            if (DeadZone != 0)
            {
                if (CircularDz)
                {
                    outputValues = _circularDeadZoneHelper.ApplyRangeDeadZone(outputValues);
                }
                else
                {
                    outputValues[0] = _deadZoneHelper.ApplyRangeDeadZone(outputValues[0]);
                    outputValues[1] = _deadZoneHelper.ApplyRangeDeadZone(outputValues[1]);
                }
                
            }
            if (Sensitivity != 100)
            {
                if (Linear)
                {
                    outputValues[0] = (short) (outputValues[0] * _linearSensitivityScaleFactor);
                    outputValues[1] = (short) (outputValues[1] * _linearSensitivityScaleFactor);
                }
                else
                {
                    outputValues[0] = _sensitivityHelper.ApplyRangeSensitivity(outputValues[0]);
                    outputValues[1] = _sensitivityHelper.ApplyRangeSensitivity(outputValues[1]);
                }
            }

            outputValues[0] = Functions.ClampAxisRange(outputValues[0]);
            outputValues[1] = Functions.ClampAxisRange(outputValues[1]);

            if (InvertX) outputValues[0] = Functions.Invert(outputValues[0]);
            if (InvertY) outputValues[1] = Functions.Invert(outputValues[1]);

            WriteOutput(0, outputValues[0]);
            WriteOutput(1, outputValues[1]);
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
