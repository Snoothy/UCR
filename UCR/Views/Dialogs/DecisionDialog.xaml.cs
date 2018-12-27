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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HidWizards.UCR.ViewModels.Dialogs;

namespace HidWizards.UCR.Views.Dialogs
{
    public partial class DecisionDialog : UserControl
    {
        public DecisionDialog(string title, string description)
        {
            DataContext = new DecisionDialogViewModel()
            {
                Title = title,
                Description = description
            };
            InitializeComponent();
        }
    }
}
