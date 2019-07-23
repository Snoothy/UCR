namespace HidWizards.UCR.Core.Models
{
    public class PropertyValidationResult
    {
        public static PropertyValidationResult ValidResult => new PropertyValidationResult(true, "");

        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public PropertyValidationResult(bool isValid, string errorMessage)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }
}
