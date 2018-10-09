﻿using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis Initializer", Disabled = true)]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisInitializer : Plugin
    {
        [PluginGui("Percentage", ColumnOrder = 0)]
        public decimal Percentage { get; set; }

        public AxisInitializer()
        {
        }
        
        public override void InitializeCacheValues()
        {
            Initialize();
        }

        public override void Update(params long[] values)
        {
        }

        private void Initialize()
        {
            var value = (long)((Percentage / 100) * Constants.AxisMaxAbsValue);
            value = Functions.ClampAxisRange(value);
            WriteOutput(0, value);
        }
    }
}
