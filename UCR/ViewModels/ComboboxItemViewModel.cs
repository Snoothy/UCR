namespace HidWizards.UCR.ViewModels
{
    public class ComboBoxItemViewModel
    {
        public string Title { get; set; }
        public dynamic Value { get; set; }

        public ComboBoxItemViewModel(string title, dynamic value)
        {
            Title = title;
            Value = value;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
