using System;
using System.Diagnostics;
using System.Threading;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to relative")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisToRelative : Plugin
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
        [PluginGui("Relative RelativeContinue", ColumnOrder = 1, RowOrder = 2)]
        public bool RelativeContinue { get; set; }

        [PluginGui("Relative Sensitivity", ColumnOrder = 2, RowOrder = 2)]
        public long RelativeSensitivity { get; set; }


        private long _currentOutputValue;
        private long _currentInputValue;
        private readonly object _threadLock = new object();
        
        private Thread _relativeThread;
        private bool _relativeThreadState = false;

        public AxisToRelative()
        {
            DeadZone = 0;
            Sensitivity = 100;
            RelativeContinue = true;
            RelativeSensitivity = 2;
        }

        public override void Update(params long[] values)
        {
            var value = values[0];

            if (Invert) value *= -1;
            if (DeadZone != 0) value = Functions.ApplyRangeDeadZone(value, DeadZone);
            if (Sensitivity != 100) value = Functions.ApplyRangeSensitivity(value, Sensitivity, Linear);

            // Respect the axis min and max ranges.
            value = Math.Min(Math.Max(value, Constants.AxisMinValue), Constants.AxisMaxValue);
            _currentInputValue = value;

            if (RelativeContinue)
            {
                lock (_threadLock)
                {
                    if (value != 0 && !_relativeThreadState)
                    {
                        SetRelativeThreadState(true);
                        //Debug.WriteLine("UCR| Started Thread");
                    }
                    else if (value == 0 && _relativeThreadState)
                    {
                        SetRelativeThreadState(false);
                        //Debug.WriteLine("UCR| Stopped Thread");
                    }
                }
            }
            else
            {
                RelativeUpdate();
            }
        }

        private void SetRelativeThreadState(bool state)
        {
            if (_relativeThreadState == state) return;
            if (!_relativeThreadState && state)
            {
                _relativeThread = new Thread(RelativeThread);
                _relativeThread.Start();
            }
            else if (_relativeThreadState && !state)
            {
                _relativeThread.Abort();
                _relativeThread.Join();
                _relativeThread = null;
            }

            _relativeThreadState = state;
        }

        public void RelativeThread()
        {
            while (RelativeContinue)
            {
                RelativeUpdate();
                Thread.Sleep(10);
            }

            lock (_threadLock)
            {
                _relativeThreadState = false;
            }
        }

        private void RelativeUpdate()
        {
            //var value = Functions.ApplyRelativeIncrement(_currentInputValue, _currentOutputValue, RelativeSensitivity);
            var value = (long)((_currentInputValue * (float)(RelativeSensitivity / 100.0)) + _currentOutputValue);
            value = Math.Min(Math.Max(value, Constants.AxisMinValue), Constants.AxisMaxValue);
            WriteOutput(0, value);
            _currentOutputValue = value;
        }
    }
}