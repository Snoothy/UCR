using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.ViewModels.Dialogs;

namespace HidWizards.UCR.Views.Dialogs
{
    public partial class StringDialog : UserControl
    {
        public string Value => ViewModel.Value;
        private StringDialogViewModel ViewModel { get; set; }

        public StringDialog(string title, string hint, string value)
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
