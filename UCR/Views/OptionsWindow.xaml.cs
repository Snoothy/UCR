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
using HidWizards.UCR.Core;
using HidWizards.UCR.ViewModels.DeviceViewModels;

namespace HidWizards.UCR.Views
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();

            if (Properties.Settings.Default.StartMinimized)
            {
                startMinimized.IsChecked = true;
            }
            else
            {
                startMinimized.IsChecked = false;
            }
        }

        private void ButtonClose_Clicked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void startMinimized_Checked(object sender, RoutedEventArgs e)
        {
        }
    }
}