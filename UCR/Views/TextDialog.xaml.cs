using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UCR.Views
{
    /// <summary>
    /// Interaction logic for TextPrompt.xaml
    /// </summary>
    public partial class TextDialog: Window
    {
        public String TextResult { get; set; }

        public TextDialog(String question, String answer="")
        {
            InitializeComponent();
            Title = question;
            //lblQuestion.Content = question + ":";
            txtAnswer.Text = answer;
        }


        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        private void BtnDialogOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAnswer.Text))
            {
                DialogResult = true;
                TextResult = txtAnswer.Text;
            }
            else
            {
                MessageBox.Show("Please fill out the field", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
        }
    }
}
