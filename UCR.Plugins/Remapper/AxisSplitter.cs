using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis Splitter")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis high")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis low")]
    public class AxisSplitter : Plugin
    {
        [PluginGui("Invert high", RowOrder = 1)]
        public bool InvertHigh { get; set; }

        [PluginGui("Invert low", RowOrder = 2)]
        public bool InvertLow { get; set; }

        [PluginGui("Dead zone")]
        public int DeadZone { get; set; }

        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();

        public AxisSplitter()
        {
            DeadZone = 0;
        }

        public override void Update(params long[] values)
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
