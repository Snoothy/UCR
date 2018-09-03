using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to Axis")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisToAxis : Plugin
    {
        [PluginGui("Invert", ColumnOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("Linear", ColumnOrder = 3)]
        public bool Linear { get; set; }

        [PluginGui("Dead zone", ColumnOrder = 1)]
        public int DeadZone { get; set; }

        [PluginGui("Sensitivity", ColumnOrder = 2)]
        public int Sensitivity { get; set; }

        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();
        private readonly SensitivityHelper _sensitivityHelper = new SensitivityHelper();

        public AxisToAxis()
        {
            DeadZone = 0;
            Sensitivity = 100;
        }

        public override void Update(params long[] values)
        {
            var value = values[0];
            if (Invert) value = Functions.Invert(value);
            if (DeadZone != 0) value = _deadZoneHelper.ApplyRangeDeadZone(value);
            if (Sensitivity != 100) value = _sensitivityHelper.ApplyRangeSensitivity(value);
            WriteOutput(0, value);
        }

        private void Initialize()
        {
            _deadZoneHelper.Percentage = DeadZone;
            _sensitivityHelper.Percentage = Sensitivity;
            _sensitivityHelper.IsLinear = Linear;
        }

        #region Event Handling
        public override void OnActivate()
        {
            base.OnActivate();
            Initialize();
        }

        public override void OnPropertyChanged()
        {
            base.OnPropertyChanged();
            Initialize();
        }
        #endregion
    }
}
