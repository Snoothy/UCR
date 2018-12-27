using System.Windows.Controls;

namespace HidWizards.UCR.Views.Dialogs
{
    public partial class BoolDialog : UserControl
    {
        public BoolDialog(string title, string description)
        {
            DataContext = new BoolDialogViewModel()
            {
                Title = title,
                Description = description
            };
            InitializeComponent();
        }
    }
}
