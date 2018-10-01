using System;
using System.Timers;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axis to Axis (Damped)")]
    [PluginInput(DeviceBindingCategory.Range, "Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class AxisToAxisDamped : Plugin
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
        /// Framerate of the smoothing simulation.
        /// High framerates make the simulation faster and more responsive,
        /// but also takes up more resources.
        /// </summary>
        [PluginGui("Refresh rate", ColumnOrder = 0, RowOrder = 2)]
        public int FPS { get; set; }

        /// <summary>
        /// Weight of an imaginary object that's attached to a spring.
        /// Heavy objects move slowly and take a while to react to sudden 
        /// changes in input. Light objects react immediately to any changes.
        /// Value should not be 0, otherwise there's a division by zero problem.
        /// </summary>
        [PluginGui("Weight", ColumnOrder = 1, RowOrder = 2)]
        public int Weight { get; set; }

        /// <summary>
        /// Damping factor. High values reduce the spring's movement a lot.
        /// Small values let the spring move for a long time. Value should
        /// be between 0% and a 100%. Other values make the simulation unstable.
        /// </summary>
        [PluginGui("Damping", ColumnOrder = 2, RowOrder = 2)]
        public int Damping { get; set; }

        private float _targetValue;
        private float _velocity;
        private float _dampedValue;
        private static Timer _simulationTimer = new Timer();
        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();
        private readonly SensitivityHelper _sensitivityHelper = new SensitivityHelper();

        public AxisToAxisDamped()
        {
            DeadZone = 0;
            Sensitivity = 100;
            FPS = 60;
            Weight = 5;
            Damping = 50;

            _velocity = 0;
            _dampedValue = 0;
            _simulationTimer.Interval = (1d / FPS) * 1000;
            _simulationTimer.Elapsed += SimulationTimerElapsed;
            SetTimerState(true);
        }

        public override void Update(params long[] values)
        {
            var value = values[0];
            if (Invert) value = Functions.Invert(value);
            if (DeadZone != 0) value = _deadZoneHelper.ApplyRangeDeadZone(value);
            if (Sensitivity != 100) value = _sensitivityHelper.ApplyRangeSensitivity(value);

            _targetValue = value;
        }

        private void SimulationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            float acceleration = _targetValue - _dampedValue;
            acceleration /= (Weight / 10f);
            _velocity = _velocity + acceleration;
            _velocity *= 1 - (Damping / 100f);

            _dampedValue += _velocity;
            _dampedValue = Math.Min(Math.Max(_dampedValue, Constants.AxisMinValue), Constants.AxisMaxValue);

            WriteOutput(0, (long)_dampedValue);
        }

        public void SetTimerState(bool state)
        {
            if (state && !_simulationTimer.Enabled)
            {
                _simulationTimer.Start();
            }
            else if (!state && _simulationTimer.Enabled)
            {
                _simulationTimer.Stop();
            }
        }

        private void Initialize()
        {
            _velocity = 0;
            _dampedValue = 0;
            _simulationTimer.Interval = (1d / FPS) * 1000;

            _deadZoneHelper.Percentage = DeadZone;
            _sensitivityHelper.Percentage = Sensitivity;
            _sensitivityHelper.IsLinear = Linear;
        }

        #region Event Handling

        public override void OnActivate()
        {
            base.OnActivate();
            Initialize();
            SetTimerState(true);
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            SetTimerState(false);
        }

        public override void OnPropertyChanged()
        {
            base.OnPropertyChanged();
            Initialize();
        }

        #endregion
    }
}