using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axes to Axes (3)")]
    [PluginInput(DeviceBindingCategory.Range, "X Axis")]
    [PluginInput(DeviceBindingCategory.Range, "Y Axis")]
	[PluginInput(DeviceBindingCategory.Range, "Z Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "X Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Y Axis")]
	[PluginOutput(DeviceBindingCategory.Range, "Z Axis")]
    public class AxesToAxes_3 : Plugin
    {
        private readonly CircularDeadZoneHelper _circularDeadZoneHelper = new CircularDeadZoneHelper();
        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();
        private readonly SensitivityHelper _sensitivityHelper = new SensitivityHelper();
        private double _linearSensitivityScaleFactor;

        [PluginGui("Invert X", ColumnOrder = 0)]
        public bool InvertX { get; set; }

        [PluginGui("Invert Y", ColumnOrder = 1)]
        public bool InvertY { get; set; }
		
		[PluginGui("Invert Z", ColumnOrder = 2)]
		public bool InvertZ { get; set; }

        [PluginGui("Sensitivity", ColumnOrder = 3)]
        public int Sensitivity { get; set; }

        [PluginGui("Linear", RowOrder = 0, ColumnOrder = 4)]
        public bool Linear { get; set; }

        [PluginGui("Dead zone", RowOrder = 1, ColumnOrder = 0)]
        public int DeadZone { get; set; }

        [PluginGui("Circular", RowOrder = 1, ColumnOrder = 2)]
        public bool CircularDz { get; set; }


        public AxesToAxes_3()
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

        public override void Update(params long[] values)
        {
            var outputValues = new long[] {values[0], values[1], values[2]};
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
					outputValues[2] = _deadZoneHelper.ApplyRangeDeadZone(outputValues[2]);
                }
                
            }
            if (Sensitivity != 100)
            {
                if (Linear)
                {
                    outputValues[0] = (long)(outputValues[0] * _linearSensitivityScaleFactor);
                    outputValues[1] = (long)(outputValues[1] * _linearSensitivityScaleFactor);
					outputValues[2] = (long)(outputValues[2] * _linearSensitivityScaleFactor);
                }
                else
                {
                    outputValues[0] = _sensitivityHelper.ApplyRangeSensitivity(outputValues[0]);
                    outputValues[1] = _sensitivityHelper.ApplyRangeSensitivity(outputValues[1]);
					outputValues[2] = _sensitivityHelper.ApplyRangeSensitivity(outputValues[2]);
                }
            }

            outputValues[0] = Functions.ClampAxisRange(outputValues[0]);
            outputValues[1] = Functions.ClampAxisRange(outputValues[1]);
			outputValues[2] = Functions.ClampAxisRange(outputValues[2]);

            if (InvertX) outputValues[0] = Functions.Invert(outputValues[0]);
            if (InvertY) outputValues[1] = Functions.Invert(outputValues[1]);
			if (InvertZ) outputValues[2] = Functions.Invert(outputValues[2]);

            WriteOutput(0, outputValues[0]);
            WriteOutput(1, outputValues[1]);
			WriteOutput(2, outputValues[2]);
        }
    }
}