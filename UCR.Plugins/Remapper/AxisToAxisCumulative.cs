using System;
using System.Diagnostics;
using System.Threading;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to Axis (Cumulative)")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisToAxisCumulative : Plugin
    {
        [PluginGui("Invert", ColumnOrder = 0)]
        public bool Invert { get; set; }

        [PluginGui("Linear", ColumnOrder = 3)]
        public bool Linear { get; set; }

        [PluginGui("Dead zone", ColumnOrder = 1)]
        public int DeadZone { get; set; }

        [PluginGui("Sensitivity", ColumnOrder = 2)]
        public int Sensitivity { get; set; }

        /// <summary>
        /// To constantly add current axis values to the output - WORK IN PROGRESS!!!
        /// </summary>
        [PluginGui("Relative Continue", ColumnOrder = 1, RowOrder = 2)]
        public bool RelativeContinue { get; set; }

        [PluginGui("Relative Sensitivity", ColumnOrder = 2, RowOrder = 2)]
        public decimal RelativeSensitivity { get; set; }

        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();
        private readonly SensitivityHelper _sensitivityHelper = new SensitivityHelper();

        private long _currentOutputValue;
        private long _currentInputValue;
        private readonly object _threadLock = new object();

        private Thread _relativeThread;

        public AxisToAxisCumulative()
        {
            DeadZone = 0;
            Sensitivity = 100;
            RelativeContinue = true;
            RelativeSensitivity = 2;
            _relativeThread = new Thread(RelativeThread);
        }

        public override void Update(params long[] values)
        {
            var value = values[0];

            if (Invert) value = Functions.Invert(value);
            if (DeadZone != 0) value = _deadZoneHelper.ApplyRangeDeadZone(value);
            if (Sensitivity != 100) value = _sensitivityHelper.ApplyRangeSensitivity(value);

            // Respect the axis min and max ranges.
            value = Math.Min(Math.Max(value, Constants.AxisMinValue), Constants.AxisMaxValue);
            _currentInputValue = value;

            if (RelativeContinue)
            {
                SetRelativeThreadState(value != 0);
            }
            else
            {
                RelativeUpdate();
            }
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
            Initialize();
        }

        public override void OnPropertyChanged()
        {
            Initialize();
        }

        public override void OnDeactivate()
        {
            SetRelativeThreadState(false);
        }
        #endregion

        private void SetRelativeThreadState(bool state)
        {
            lock (_threadLock)
            {
                var relativeThreadActive = RelativeThreadActive();
                if (!relativeThreadActive && state)
                {
                    _relativeThread = new Thread(RelativeThread);
                    _relativeThread.Start();
                    Debug.WriteLine("UCR| Started Relative Thread");
                }
                else if (relativeThreadActive && !state)
                {
                    _relativeThread.Abort();
                    _relativeThread.Join();
                    _relativeThread = null;
                    Debug.WriteLine("UCR| Stopped Relative Thread");
                }
            }
        }

        private bool RelativeThreadActive()
        {
            return _relativeThread != null && _relativeThread.IsAlive;
        }

        public void RelativeThread()
        {
            while (RelativeContinue)
            {
                RelativeUpdate();
                Thread.Sleep(10);
            }
        }

        private void RelativeUpdate()
        {
            var value = (long)((_currentInputValue * (RelativeSensitivity / 100)) + _currentOutputValue);
            value = Math.Min(Math.Max(value, Constants.AxisMinValue), Constants.AxisMaxValue);
            WriteOutput(0, value);
            _currentOutputValue = value;
        }
    }
}