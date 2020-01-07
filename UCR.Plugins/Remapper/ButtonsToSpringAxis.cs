using System;
using System.Reflection;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Buttons to Spring Axis", Group = "Axis", Description = "Map from one button to another", FixedUpdate = true)]
    [PluginInput(DeviceBindingCategory.Momentary, "Button (Low)")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button (High)")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    [PluginGroup(Group = "Spring", Name = "Spring")]
    public class ButtonsToSpringAxis : Plugin
    {
        [PluginGui("Resting point")]
        public double Start { get; set; }

        [PluginGui("Low target point")]
        public double LowTarget { get; set; }

        [PluginGui("High target point")]
        public double HighTarget { get; set; }

        /// <summary>
        /// Weight of an imaginary object that's attached to a spring.
        /// Heavy objects move slowly and take a while to react to sudden 
        /// changes in input. Light objects react immediately to any changes.
        /// Value should not be 0, otherwise there's a division by zero problem.
        /// </summary>
        [PluginGui("Weight", Group = "Spring")]
        public int Weight { get; set; }

        /// <summary>
        /// Damping factor. High values reduce the spring's movement a lot.
        /// Small values let the spring move for a long time. Value should
        /// be between 0% and a 100%. Other values make the simulation unstable.
        /// </summary>
        [PluginGui("Damping", Group = "Spring")]
        public int Damping { get; set; }

        private float _targetValue;
        private float _velocity;
        private float _dampedValue;

        public ButtonsToSpringAxis()
        {
            Start = 0.0;
            LowTarget = -100.0;
            HighTarget = 100.0;
            Weight = 100;
            Damping = 50;

            _velocity = 0;
            _dampedValue = 0;
        }

        public override void Update(params short[] values)
        {
            if (values[0] == 1 && values[1] == 1)
            {
                _targetValue = Functions.GetRangeFromPercentage(Start);
            }
            else if (values[0] == 1)
            {
                _targetValue = Functions.GetRangeFromPercentage(LowTarget);
            }
            else if (values[1] == 1)
            {
                _targetValue = Functions.GetRangeFromPercentage(HighTarget);
            }
            else
            {
                _targetValue = Functions.GetRangeFromPercentage(Start);
            }
        }

        public override void FixedUpdate(long deltaMillis)
        {
            var acceleration = _targetValue - _dampedValue;
            acceleration /= (Weight / 10f);
            _velocity += acceleration;
            _velocity *= 1 - (Damping / 100f);

            _dampedValue += _velocity * (deltaMillis/10f);
            _dampedValue = Math.Min(Math.Max(_dampedValue, Constants.AxisMinValue), Constants.AxisMaxValue);

            WriteOutput(0, (short)_dampedValue);
        }

        private void Initialize()
        {
            _velocity = 0;
            _dampedValue = 0;
            _targetValue = Functions.GetRangeFromPercentage(Start);
        }

        public override void OnActivate()
        {
            Initialize();
        }

        public override void OnPropertyChanged()
        {
            Initialize();
        }

        public override PropertyValidationResult Validate(PropertyInfo propertyInfo, dynamic value)
        {
            switch (propertyInfo.Name)
            {
                case nameof(Start):
                case nameof(LowTarget):
                case nameof(HighTarget):
                    return InputValidation.ValidateRange(value, -100, 100);
                case nameof(Damping):
                    return InputValidation.ValidatePercentage(value);
            }

            return PropertyValidationResult.ValidResult;
        }
    }
}
