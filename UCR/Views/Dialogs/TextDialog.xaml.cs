using System;
using System.Windows;

namespace HidWizards.UCR.Views.Dialogs
{
    public partial class TextDialog: Window
    {
        public string TextResult { get; set; }

        public TextDialog(string question, string answer="")
        {
            InitializeComponent();
            Title = question;
            TxtAnswer.Text = answer;
        }


        private void Window_ContentRendered(object sender, EventArgs e)
        {
            TxtAnswer.SelectAll();
            TxtAnswer.Focus();
        }

        private void BtnDialogOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtAnswer.Text))
            {
                DialogResult = true;
                TextResult = TxtAnswer.Text;
            }
            else
            {
                MessageBox.Show("Please fill out the field", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
        }
    }
}
