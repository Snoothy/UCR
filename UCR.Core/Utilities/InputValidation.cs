using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.Core.Utilities
{
    public static class InputValidation
    {
        public static PropertyValidationResult ValidatePercentage(double value)
        {
            return ValidateRange(value, 0.0, 100.0);
        }

        public static PropertyValidationResult ValidateRange(double value, double min, double max)
        {
            if (value > max) return new PropertyValidationResult(false, $"Value must be {max} or less");
            if (value < min) return new PropertyValidationResult(false, $"Value must be {min} or more");

            return PropertyValidationResult.ValidResult;
        }

        public static PropertyValidationResult ValidateNotZero(double value)
        {
            if (value.Equals(0.0)) return new PropertyValidationResult(false, $"Value cannot be 0");

            return PropertyValidationResult.ValidResult;
        }
    }
}