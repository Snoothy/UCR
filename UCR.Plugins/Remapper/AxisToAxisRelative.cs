﻿using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to axis (Relative)")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisToAxisRelative : Plugin
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

        [PluginGui("Use delta", ColumnOrder = 3, RowOrder = 2)]
        public bool UseDelta { get; set; }


        [PluginGui("Multiplier", ColumnOrder = 4, RowOrder = 2)]
        public int Multiplier { get; set; }

        private long _axisRest;

        private long _currentOutputValue;
        private long _currentInputValue;
        [NotNull] private readonly object _threadLock = new object();

        [NotNull] private Thread _relativeThread;

        public AxisToAxisRelative()
        {
            DeadZone = 0;
            Sensitivity = 100;
            RelativeContinue = true;
            RelativeSensitivity = 2;
            UseDelta = true;
            Multiplier = 20;
            _axisRest = 0;
            _relativeThread = new Thread(RelativeThread);
        }
        
        public override void Update(params long[] values)
        {
            long value;

            var raw = values[0];

            // Use either raw input or calculated delta
            if (UseDelta)
            {
                // delta
                var delta = raw - _axisRest;
                // Alter the response
                delta = delta * Multiplier;

                value = delta;
                Debug.WriteLine($"Input Delta: {value}");
            }
            else
            {
                value = raw;
                Debug.WriteLine($"Raw Input: {raw}");
            }

            // Transform the input as needed
            if (Invert) value *= -1;
            if (DeadZone != 0) value = Functions.ApplyRangeDeadZone(value, DeadZone);
            if (Sensitivity != 100) value = Functions.ApplyRangeSensitivity(value, Sensitivity, Linear);

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
                
                Debug.WriteLine($"Relative Output: {value}");

            _axisRest = raw;
        }

        public override void OnDeactivate()
        {
            SetRelativeThreadState(false);
        }

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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_threadLock != null);
            Contract.Invariant(_relativeThread != null);
        }
    }
}