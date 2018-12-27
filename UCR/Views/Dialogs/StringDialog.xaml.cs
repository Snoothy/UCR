using System.Windows;
using System.Windows.Controls;

namespace HidWizards.UCR.Views.Dialogs
{
    public partial class SimpleDialog : UserControl
    {
        public string Value => ViewModel.Value;
        private StringDialogViewModel ViewModel { get; set; }

        public SimpleDialog(string title, string hint, string value)
        {
            ViewModel = new StringDialogViewModel()
            {
                Title = title,
                Hint = hint,
                Value = value
            };
            DataContext = ViewModel;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TextValue.SelectAll();
        }
    }
}
